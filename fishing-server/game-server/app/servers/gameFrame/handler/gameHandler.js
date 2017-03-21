/**
 * 
 * 游戏具体逻辑处理机
 */
var gameManager = require('../../../domain/game/gameManager.js');

module.exports = function(app) {
  return new Handler(app);
};

var Handler = function(app) {
  this.app = app;
};

var handler = Handler.prototype;

/*
* 出钩通知
* 客户端通过'gameFrame.gameHandler.reqShoot'通知
* 参数： 1、angle   出钩角度
*
* */
handler.reqShoot = function(msg, session, next) {
  gameManager.shoot(session.uid, msg.angle, function(err, res) {
    next(err, res);
  });
};

/*
* 钩子碰撞到鱼
* 客户端通过'gameFrame.gameHandler.reqTouchFish'请求
* 参数： 1、fishId  鱼的id
* */
handler.reqTouchFish = function(msg, session, next) {
  gameManager.touchFish(session.uid, msg.fishId, function(err, res) {
    next(err, res);
  });
};

/*
* 钓鱼结果通知
* 客户端通过 'gameFrame.gameHandler.reqFishResult' 请求
* 参数： 1、fishId  鱼的id
*        2、isCatch 是否捕获(Boolean)
* */
handler.reqFishResult = function(msg, session, next) {
  gameManager.fishResult(session.uid, msg.fishId, msg.isCatch, function(err, res) {
    next(err, res);
  });
};

/*
* 请求切换鱼钩
* 客户端通过 'gameFrame.gameHandler.reqChangeHook' 请求
* */
handler.reqChangeHook = function(msg, session, next) {
  gameManager.changeHook(session.uid, msg.hook, function(err, res) {
    next(err, res);
  });
};