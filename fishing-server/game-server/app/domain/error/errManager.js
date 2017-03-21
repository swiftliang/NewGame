/**
 * 
 * handler错误处理逻辑
 */
var pomelo = require('pomelo');

// err是前置过滤器或处理机传给的错误对象，resp是处理机本来将要传给客户端的响应消息
// 一旦handler有错误，则会执行到这
module.exports = function(err, msg, res, session, next) {
  pomelo.app.logger.error(err.message + '  -----  uid: ' + session.uid + '\n' + err.stack);
  next(null, res);
};