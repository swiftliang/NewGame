/**
 * 
 * 聊天系统处理机
 */
var chatManager = require('../../../domain/game/chatManager.js');

module.exports = function(app) {
  return new Handler(app);
};

var Handler = function(app) {
  this.app = app;
};

var handler = Handler.prototype;

/*
* 聊天请求
* 客户端通过 'gameFrame.chatHandler.reqChat' 请求
* */
handler.reqChat = function(msg, session, next) {
  chatManager.chat(session.uid, msg.type, msg.message, function(err, res) {
    next(err, res);
  });
};