/**
 * 
 * 玩家对象
 */
module.exports = function(opts) {
  return new player(opts);
};

var player = function(opts) {
  this.uid = opts.uid;                              //玩家id
  this.sid = opts.sid;                              //玩家连接的前端服务器id
  this.tableId = opts.tableId || 0;                 //玩家所在的频道(桌子)
  this.chairId = opts.chairId || 0;                 //玩家所在的椅子id
  this.gold = opts.gold || 0;                       //金币数量
  this.lottery = opts.lottery || 0;                 //彩票数量
  this.photoId = opts.photoId || 0;                 //头像
  this.level = opts.level || 0;                     //等级
  this.exp = opts.exp || 0;                         //经验
  this.nickName = opts.nickname;                    //昵称
  this.hook = opts.hook || 1;                       //钩子状态，默认为1
};