(window.webpackJsonp=window.webpackJsonp||[]).push([[14],{"CA+f":function(P,u,t){"use strict";t.r(u);var m=t("k1fw"),E=t("WmNS"),n=t.n(E),i=t("9og8"),l=t("tJVT"),T=t("tMyG"),g=t("Qiat"),j=t("q1tI"),f=t("9kvl"),I=t("sy1d");function h(o){return b.apply(this,arguments)}function b(){return b=Object(i.a)(n.a.mark(function o(a){return n.a.wrap(function(s){for(;;)switch(s.prev=s.next){case 0:return console.log(a),s.abrupt("return",Object(I.a)("/syslog/search",{params:a}));case 2:case"end":return s.stop()}},o)})),b.apply(this,arguments)}var v=t("Y/bI"),A=t("nKUr"),O=function(){var a=Object(f.h)(),r=Object(j.useState)(),s=Object(l.a)(r,2),e=s[0],D=s[1],U=function(){var c=Object(i.a)(n.a.mark(function p(){var C,y;return n.a.wrap(function(d){for(;;)switch(d.prev=d.next){case 0:return d.next=2,Object(v.f)({});case 2:return C=d.sent,y={},C.data.forEach(function(S){y[S.id]={text:S.name}}),d.abrupt("return",y);case 6:case"end":return d.stop()}},p)}));return function(){return c.apply(this,arguments)}}();Object(j.useEffect)(function(){U().then(function(c){console.log("app enums ",c),D(Object(m.a)({},c))})},[]);var w=[{title:a.formatMessage({id:"pages.logs.table.appname"}),dataIndex:"appId",valueType:"select",valueEnum:e},{title:a.formatMessage({id:"pages.logs.table.type"}),dataIndex:"logType",valueType:"select",valueEnum:{0:{text:a.formatMessage({id:"pages.logs.table.type.0"})},1:{text:a.formatMessage({id:"pages.logs.table.type.1"})}}},{title:a.formatMessage({id:"pages.logs.table.time"}),dataIndex:"logTime",valueType:"dateTime",hideInSearch:!0},{title:a.formatMessage({id:"pages.logs.table.time"}),dataIndex:"logTime",valueType:"dateRange",hideInTable:!0,search:{transform:function(p){return{startTime:p[0],endTime:p[1]}}}},{title:a.formatMessage({id:"pages.logs.table.content"}),dataIndex:"logText",hideInSearch:!0}];return Object(A.jsx)(T.a,{children:Object(A.jsx)(g.a,{options:!1,search:{labelWidth:"auto"},rowKey:"id",columns:w,request:function(p,C,y){return h(Object(m.a)({},p))}})})},M=u.default=O},"Y/bI":function(P,u,t){"use strict";t.d(u,"f",function(){return T}),t.d(u,"a",function(){return j}),t.d(u,"c",function(){return I}),t.d(u,"b",function(){return b}),t.d(u,"e",function(){return A}),t.d(u,"d",function(){return M});var m=t("k1fw"),E=t("WmNS"),n=t.n(E),i=t("9og8"),l=t("sy1d");function T(a){return g.apply(this,arguments)}function g(){return g=Object(i.a)(n.a.mark(function a(r){return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",Object(l.a)("/app/search",{params:r}));case 1:case"end":return e.stop()}},a)})),g.apply(this,arguments)}function j(a){return f.apply(this,arguments)}function f(){return f=Object(i.a)(n.a.mark(function a(r){return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",Object(l.a)("/app/add",{method:"POST",data:Object(m.a)({},r)}));case 1:case"end":return e.stop()}},a)})),f.apply(this,arguments)}function I(a){return h.apply(this,arguments)}function h(){return h=Object(i.a)(n.a.mark(function a(r){return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",Object(l.a)("/app/edit",{method:"POST",data:Object(m.a)({},r)}));case 1:case"end":return e.stop()}},a)})),h.apply(this,arguments)}function b(a){return v.apply(this,arguments)}function v(){return v=Object(i.a)(n.a.mark(function a(r){return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",Object(l.a)("/app/delete",{method:"POST",params:{id:r.id}}));case 1:case"end":return e.stop()}},a)})),v.apply(this,arguments)}function A(a){return O.apply(this,arguments)}function O(){return O=Object(i.a)(n.a.mark(function a(r){return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",Object(l.a)("/app/InheritancedApps",{params:{currentAppId:r}}));case 1:case"end":return e.stop()}},a)})),O.apply(this,arguments)}function M(a){return o.apply(this,arguments)}function o(){return o=Object(i.a)(n.a.mark(function a(r){return n.a.wrap(function(e){for(;;)switch(e.prev=e.next){case 0:return e.abrupt("return",Object(l.a)("/app/DisableOrEanble",{method:"POST",params:{id:r}}));case 1:case"end":return e.stop()}},a)})),o.apply(this,arguments)}}}]);