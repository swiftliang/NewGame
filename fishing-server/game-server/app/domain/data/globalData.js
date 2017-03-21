/**
 * 
 * 内存中缓存的全局数据管理模块
 */
var CONSTS = require('../../consts/consts.js');

module.exports = function(app) {
  return new GlobalData(app);
};

var GlobalData = function(app) {
  this.players = {};                          //玩家信息map
  //this.tables = {'88': {tableId: 88, left: null, right: null}, '99': {tableId: 99, left: null, right: null}};
  this.tables = {};
  for(var i = 1; i < 100; i++) {
    this.tables[i] = {tableId: i, left: null, right: null};
  }
  //this.freeTables = {'88': {tableId: 88, left: null, right: null}, '99': {tableId: 99, left: null, right: null}};                     //免费场桌子信息map
  //this.chargeTables = {};                     //收费场桌子信息map
  this.fishs = {};                            //桌子上的出鱼信息
  this.timers = {};                           //桌子上的定时器
  this.tableInitTime = {};                    //桌子创建时间
};

var globalData = GlobalData.prototype;

/*
 * 新增玩家信息缓存数据
 * */
globalData.addPlayer = function(player) {
  this.players[player.uid] = player;
};

/*
 * 移除玩家缓存数据
 * */
globalData.removePlayer = function(uid) {
  delete this.players[uid];
};

/*
 * 获取桌子列表数据
 * */
//globalData.getTables = function(mode) {
//  var tableData;
//  switch(mode) {
//    case CONSTS.MODE.FREE:
//      tableData = this.freeTables;
//      break;
//    case CONSTS.MODE.CHARGE:
//      tableData = this.chargeTables;
//      break;
//  }
//
//  return tableData;
//};

/*
 * 设置桌子列表数据
 * */
//globalData.setTables = function(mode, tableId, data) {
//  switch(mode) {
//    case CONSTS.MODE.FREE:
//      this.freeTables[tableId] = data;
//      break;
//    case CONSTS.MODE.CHARGE:
//      this.chargeTables[tableId] = data;
//      break;
//  }
//};

/*
 * 移除桌子信息数据
 * */
globalData.removeTable = function(tableId) {
  delete this.fishs[tableId];
  //if(this.freeTables[tableId]) {
  //  this.freeTables[tableId].left = null;
  //  this.freeTables[tableId].right = null;
  //}
  //if(this.chargeTables[tableId]) {
  //  this.chargeTables[tableId].left = null;
  //  this.chargeTables[tableId].right = null;
  //}

  this.tables[tableId].left = null;
  this.tables[tableId].right = null;

  clearInterval(this.timers[tableId]);
  delete this.timers[tableId];
  delete this.tableInitTime[tableId];
};

/*
 * 清空椅子信息
 * */
globalData.clearChair = function(tableId, chairId) {
  //if(this.freeTables[tableId]) {
  //  this.freeTables[tableId][chairId] = null;
  //}
  //if(this.chargeTables[tableId]) {
  //  this.chargeTables[tableId][chairId] = null;
  //}

  this.tables[tableId][chairId] = null;
};

/*
 * 清理过期的鱼的缓存
 * */
globalData.clearFish = function(tableId, fishId) {
  delete this.fishs[tableId][fishId];
};