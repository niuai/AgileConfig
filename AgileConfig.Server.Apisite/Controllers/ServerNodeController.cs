﻿using System;
using System.Threading.Tasks;
using AgileConfig.Server.Apisite.Filters;
using AgileConfig.Server.Apisite.Models;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.IService;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using AgileConfig.Server.Common;

namespace AgileConfig.Server.Apisite.Controllers
{
    [Authorize]
    [ModelVaildate]
    public class ServerNodeController : Controller
    {
        private readonly IServerNodeService _serverNodeService;
        private readonly ISysLogService _sysLogService;
        private readonly IRemoteServerNodeProxy _remoteServerNodeProxy;

        public ServerNodeController(IServerNodeService serverNodeService,
            ISysLogService sysLogService,
            IRemoteServerNodeProxy remoteServerNodeProxy)
        {
            _serverNodeService = serverNodeService;
            _sysLogService = sysLogService;
            _remoteServerNodeProxy = remoteServerNodeProxy;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]ServerNodeVM model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.Address = model.Address.TrimEnd('/');
            var oldNode = await _serverNodeService.GetAsync(model.Address);
            if (oldNode != null)
            {
                return Json(new
                {
                    success = false,
                    message = "节点已存在，请重新输入。"
                });
            }

            var node = new ServerNode();
            node.Address = model.Address.TrimEnd('/');
            node.Remark = model.Remark;
            node.Status = NodeStatus.Offline;
            node.CreateTime = DateTime.Now;

            var result = await _serverNodeService.AddAsync(node);
            if (result)
            {
                TinyEventBus.Instance.Fire(EventKeys.ADD_SYSLOG, new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    LogText = $"新增节点：{node.Address}"
                });
                await _remoteServerNodeProxy.TestEchoAsync(node.Address);
            }
           
            return Json(new
            {
                data = node,
                success = result,
                message = !result ? "添加节点失败，请查看错误日志" : ""
            });
        }


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]ServerNodeVM model)
        {
            if (Appsettings.IsPreviewMode)
            {
                return Json(new
                {
                    success = false,
                    message = "演示模式请勿删除节点"
                });
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var node = await _serverNodeService.GetAsync(model.Address);
            if (node == null)
            {
                return Json(new
                {
                    success = false,
                    message = "未找到对应的节点。"
                });
            }

            var result = await _serverNodeService.DeleteAsync(node);
            if (result)
            {
                TinyEventBus.Instance.Fire(EventKeys.ADD_SYSLOG, new SysLog
                {
                    LogTime = DateTime.Now,
                    LogType = SysLogType.Normal,
                    LogText = $"删除节点：{node.Address}"
                });
            }
            return Json(new
            {
                success = result,
                message = !result ? "删除节点失败，请查看错误日志" : ""
            });
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var nodes = await _serverNodeService.GetAllNodesAsync();

            return Json(new
            {
                success = true,
                data = nodes.OrderBy(n => n.CreateTime)
            });
        }
    }
}
