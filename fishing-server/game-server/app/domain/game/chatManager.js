/**
 * 
 * 聊天处理逻辑
 */
var pomelo = require('pomelo'),
    CODE = require('../../consts/code.js'),
    CONSTS = require('../../consts/consts.js'),
    ROUTE  = require('../../consts/route.js'),
    channelService = require('../../service/channelService.js');

var chatManager = module.exports;

/*
* 聊天请求处理逻辑
* */
chatManager.chat = function(uid, type, message, cb) {
  var player = pomelo.app.globalData.players[uid];
  // 玩家不在桌子上，不可以聊天
  if(!player.tableId) {
    return cb(new Error(CONSTS.ERR.LOGIC_ERR), {code: CODE.DATA_ERROR});
  }

  channelService.pushByChannel(player.tableId, ROUTE.CHAT, {uid: uid, nickName: player.nickName, type: type, message: message});
};