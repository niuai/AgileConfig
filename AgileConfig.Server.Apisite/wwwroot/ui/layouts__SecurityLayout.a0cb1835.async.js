(window.webpackJsonp=window.webpackJsonp||[]).push([[7],{"0jlH":function(M,u,e){"use strict";e.r(u);var m=e("fWQN"),c=e("mtLc"),g=e("yKVA"),E=e("879j"),O=e("q1tI"),C=e.n(O),f=e("95SA"),v=e("Ty5D"),h=e("9kvl"),a=e("nKUr"),P=e.n(a),A=function(o){Object(g.a)(_,o);var s=Object(E.a)(_);function _(){var i;Object(m.a)(this,_);for(var n=arguments.length,r=new Array(n),t=0;t<n;t++)r[t]=arguments[t];return i=s.call.apply(s,[this].concat(r)),i.state={isReady:!1},i}return Object(c.a)(_,[{key:"componentDidMount",value:function(){this.setState({isReady:!0});var n=this.props.dispatch;n&&n({type:"user/fetchCurrent"})}},{key:"render",value:function(){var n=this.state.isReady,r=this.props,t=r.children,D=r.loading,l=r.currentUser,d=l&&l.userid;return!d&&D||!n?Object(a.jsx)(f.a,{}):!d&&window.location.pathname!=="/user/login"?Object(a.jsx)(v.c,{to:"/user/login"}):t}}]),_}(C.a.Component);u.default=Object(h.c)(function(o){var s=o.user,_=o.loading;return{currentUser:s.currentUser,loading:_.models.user}})(A)}}]);