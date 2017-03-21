/*
* gate服务器处理机
* */
var CODE = require('../../../consts/code.js'),
      dispatcher = require('../../../util/dispatcher');

module.exports = function(app) {
   return new Handler(app);
};

var Handler = function(app) {
   this.app = app;
   this.count = 1;
};

var handler = Handler.prototype;

/*
 * gateHandler的queryEntry方法，主要负责负载均衡
 * 客户端通过'gate.gateHandler.queryEntry'请求
 * 为连接gate的客户端选择一个合适的connector地址(host、port)
 * 返回connector地址
 * */
handler.queryEntry = function(msg, session, next) {
   //console.log('------ gate ------: ' + msg.uid);
   var uid = msg.uid;
   // 先检查uid有效性，无效则返回错误状态码
   if(!uid) {
      next(null, {code: CODE.FAILED});
      return;
   }
   // 获取所有的connector，并判断有效性，无效则返回错误代码
   var connectors = this.app.getServersByType('connector');
   if(!connectors || connectors.length === 0) {
      next(null, {code: CODE.FAILED});
      return;
   }

   // 调用dispatcher.dispatch()方法，选择合适的connector地址返回给客户端
   //var res = dispatcher.dispatch(uid, connectors);

   // 选择合适的connector地址返回给客户端
   var res  = dispatcher.dispatchByCount(this.count, connectors);
   this.count++;

   next(null, {
      code: CODE.SUCCESS,
      host: res.clientHost,
      port: res.clientPort
   });
};