﻿using System;
using System.Threading.Tasks;
using AgileConfig.Server.Apisite.Filters;
using AgileConfig.Server.Apisite.Models;
using AgileConfig.Server.Data.Entity;
using AgileConfig.Server.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;
using AgileConfig.Server.Common;

namespace AgileConfig.Server.Apisite.Controllers
{
    [Authorize]
    [ModelVaildate]
    public class AppController : Controller
    {
        private readonly IAppService _appService;

        public AppController(IAppService appService)
        {
            _appService = appService;
        }

        public async Task<IActionResult> Search(string name, string id, int current = 1, int pageSize = 20)
        {
            if (current < 1)
            {
                throw new ArgumentException("current cant less then 1 .");
            }
            if (pageSize < 1)
            {
                throw new ArgumentException("pageSize cant less then 1 .");
            }

            var all = await _appService.GetAllAppsAsync();
            if (!string.IsNullOrWhiteSpace(name))
            {
                all = all.Where(x => x.Name.Contains(name)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                all = all.Where(x => x.Id.Contains(id)).ToList();
            }

            var count = all.Count;
            var pageList = all.OrderBy(x => x.CreateTime).ToList().Skip((current - 1) * pageSize).Take(pageSize);
            var vms = new List<AppListVM>();
            foreach (var item in pageList)
            {
                var inheritancedApps = await _appService.GetInheritancedAppsAsync(item.Id);
                vms.Add(new AppListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Secret = item.Secret,
                    Inheritanced = item.Type == AppType.Inheritance,
                    Enabled = item.Enabled,
                    UpdateTime = item.UpdateTime,
                    CreateTime = item.CreateTime,
                    inheritancedApps = item.Type == AppType.Inheritance ? 
                                                                            new List<string>() : 
                                                                            (inheritancedApps).Select(ia => ia.Id).ToList(),
                    inheritancedAppNames = item.Type == AppType.Inheritance ?
                                                                            new List<string>() :
                                                                            (inheritancedApps).Select(ia => ia.Name).ToList()
                });
            }

            return Json(new
            {
                current,
                pageSize,
                success = true,
                total = count,
                data = vms
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AppVM model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var oldApp = await _appService.GetAsync(model.Id);
            if (oldApp != null)
            {

                return Json(new
                {
                    success = false,
                    message = "应用Id已存在，请重新输入。"
                });
            }

            var app = new App();
            app.Id = model.Id;
            app.Name = model.Name;
            app.Secret = model.Secret;
            app.Enabled = model.Enabled;
            app.CreateTime = DateTime.Now;
            app.UpdateTime = null;
            app.Type = model.Inheritanced ? AppType.Inheritance : AppType.PRIVATE;

            var inheritanceApps = new List<AppInheritanced>();
            if (!model.Inheritanced && model.inheritancedApps != null)
            {
                var sort = 0;
                model.inheritancedApps.ForEach(appId =>
                {
                    inheritanceApps.Add(new AppInheritanced
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        AppId = app.Id,
                        InheritancedAppId = appId,
                        Sort = sort++
                    });
                });
            }

            var result = await _appService.AddAsync(app, inheritanceApps);
            if (result)
            {
                TinyEventBus.Instance.Fire(EventKeys.ADD_APP_SUCCESS, app);
            }

            return Json(new
            {
                data = app,
                success = result,
                message = !result ? "新建应用失败，请查看错误日志" : ""
            });
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] AppVM model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var app = await _appService.GetAsync(model.Id);
            if (app == null)
            {
                return Json(new
                {
                    success = false,
                    message = "未找到对应的应用程序。"
                });
            }

            if (Appsettings.IsPreviewMode && app.Name == "test_app")
            {
                return Json(new
                {
                    success = false,
                    message = "演示模式请勿修改Test_App"
                });
            }

            app.Name = model.Name;
            app.Secret = model.Secret;
            app.Enabled = model.Enabled;
            app.UpdateTime = DateTime.Now;
            app.Type = model.Inheritanced ? AppType.Inheritance : AppType.PRIVATE;
            var inheritanceApps = new List<AppInheritanced>();
            if (!model.Inheritanced && model.inheritancedApps != null)
            {
                var sort = 0;
                model.inheritancedApps.ForEach(appId =>
                {
                    inheritanceApps.Add(new AppInheritanced
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        AppId = app.Id,
                        InheritancedAppId = appId,
                        Sort = sort++
                    });
                });
            }

            var result = await _appService.UpdateAsync(app, inheritanceApps);
            if (result)
            {
                TinyEventBus.Instance.Fire(EventKeys.EDIT_APP_SUCCESS, app);
            }
            return Json(new
            {
                success = result,
                message = !result ? "修改应用失败，请查看错误日志" : ""
            });
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var apps = await _appService.GetAllAppsAsync();
            var vms = new List<AppListVM>();
            foreach (var item in apps)
            {
                vms.Add(new AppListVM
                {
                    Id = item.Id,
                    Name = item.Name,
                    Secret = item.Secret,
                    Inheritanced = item.Type == AppType.Inheritance,
                    Enabled = item.Enabled,
                    UpdateTime = item.UpdateTime,
                    CreateTime = item.CreateTime,
                    inheritancedApps = item.Type == AppType.Inheritance ?
                                                                            new List<string>() :
                                                                            (await _appService.GetInheritancedAppsAsync(item.Id)).Select(ia => ia.Id).ToList()
                });
            }

            return Json(new
            {
                success = true,
                data = vms
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var app = await _appService.GetAsync(id);
            var vm = new AppVM();
            vm.Id = app.Id;
            vm.Name = app.Name;
            vm.Secret = app.Secret;
            vm.Inheritanced = app.Type == AppType.Inheritance;
            vm.Enabled = app.Enabled;

            vm.inheritancedApps = (await _appService.GetInheritancedAppsAsync(id)).Select(x => x.Id).ToList();

            return Json(new
            {
                success = app != null,
                data = vm,
                message = app == null ? "未找到对应的应用程序。" : ""
            });
        }

        /// <summary>
        /// 在启动跟禁用之间进行切换
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DisableOrEanble(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var app = await _appService.GetAsync(id);
            if (app == null)
            {
                return Json(new
                {
                    success = false,
                    message = "未找到对应的应用程序。"
                });
            }

            app.Enabled = !app.Enabled;

            var result = await _appService.UpdateAsync(app);

            if (result)
            {
                TinyEventBus.Instance.Fire(EventKeys.DISABLE_OR_ENABLE_APP_SUCCESS, app);
            }

            return Json(new
            {
                success = result,
                message = !result ? "修改应用失败，请查看错误日志" : ""
            });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            var app = await _appService.GetAsync(id);
            if (app == null)
            {
                return Json(new
                {
                    success = false,
                    message = "未找到对应的应用程序。"
                });
            }

            var result = await _appService.DeleteAsync(app);

            if (result)
            {
                TinyEventBus.Instance.Fire(EventKeys.DELETE_APP_SUCCESS, app);
            }

            return Json(new
            {
                success = result,
                message = !result ? "修改应用失败，请查看错误日志" : ""
            });
        }

        /// <summary>
        /// 获取所有可以继承的app
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InheritancedApps(string currentAppId)
        {
            var apps = await _appService.GetAllInheritancedAppsAsync();
            apps = apps.Where(a => a.Enabled).ToList();
            var self = apps.FirstOrDefault(a => a.Id == currentAppId);
            if (self != null)
            {
                //过滤本身
                apps.Remove(self);
            }
            var vms = apps.Select(x =>
            {
                return new
                {
                    Id = x.Id,
                    Name = x.Name,
                };
            });

            return Json(new
            {
                success = true,
                data = vms
            });
        }
    }
}
