/**
 * 
 * 大厅桌子请求处理机
 */
var CODE = require('../../../consts/code.js'),
    tableManager = require('../../../domain/table/tableManager.js');

module.exports = function(app) {
  return new Handler(app);
};

var Handler = function(app) {
  this.app = app;
};

var handler = Handler.prototype;

/*
* 选择游戏模式
* 客户端通过'gameFrame.tableHandler.reqSelectMode'请求
* 参数：   1、mode(free/charge)： 游戏模式(免费/付费)————目前取消了模式选择
* */
handler.reqSelectMode = function(msg, session, next) {
  tableManager.selectMode(function(err, res) {
    next(err, res);
  });
};

/*
* 选择某张桌子的一个位置坐下
* 客户端通过'gameFrame.tableHandler.reqSitDown'请求
* */
handler.reqSitDown = function(msg, session, next) {
  tableManager.sitDown(session.uid, msg.tableId, msg.chairId, function(err, res) {
    next(err, res);
  });
};

/*
* 成功载入游戏场景后的请求
* 客户端通过 'gameFrame.tableHandler.ntfLoadComplete' 请求
* */
handler.ntfLoadComplete = function(msg, session, next) {
  tableManager.loadComplete(session.uid);
  next(null, null);
};

/*
* 离开桌子的请求
* 客户端通过 'gameFrame.tableHandler.reqLeaveTable'请求
* */
handler.reqLeaveTable = function(msg, session, next) {
  tableManager.leaveTable(session.uid, msg.tableId, function(err, res) {
    next(null, res);
  });
};

/*
* 快速开始游戏请求
* 客户端通过 'gameFrame.tableHandler.reqQuickStart'请求
* */
handler.reqQuickStart = function(msg, session, next) {
  tableManager.quickStart(session.uid, msg.mode, function(err, res) {
    next(err, res);
  });
};