/**
 * 
 * 这个模块主要处理涉及到的各种数据库操作
 */
var sqlClient = require('../dao/mysql.js');
var mysqlConfig = require('../../shared/config/mysql.json');
var dataBase = new sqlClient(mysqlConfig);
var logger = require('./logger.js').logger;
var crypto = require('./crypto');
var dbService = module.exports;

/*
 *  从玩家账号库查找玩家账号信息
 * */
dbService.selectUserinfo = function(userName, callback) {
  var sql = 'select id, userName, password from userinfo where userName = ?';
  var args = [userName];
  dataBase.query(sql, args, function(err, res) {
    if(err) {
      logger.error('selectUserinfo error: ' + err);
      callback(false);
      return;
    }
    callback(res);
  });
};

/*
 *  将玩家的注册信息写入数据库
 * */
dbService.registerUserinfo = function(userName, password, deviceID, registerType, callback) {
  var sql = 'insert into userinfo (userName, password, registerType, registerDeviceID, registerTime) values (?, ?, ?, ?, ?)';
  var args = [userName, crypto.md5(password), registerType, deviceID, Date.now()];
  dataBase.query(sql, args, function(err, res) {
    if(err) {
      logger.error('registerUserinfo error: ' + err);
      callback(false);
      return;
    }
    callback(true);
  });
};

/*
* 玩家登陆成功后更新玩家的登录时间
* */
dbService.updateUserLoginTime = function(userName) {
  var sql = 'update userinfo set lastLoginTime = ? where userName = ?';
  var args = [Date.now(), userName];
  dataBase.query(sql, args, function(err, res) {
    if(err) {
      logger.error('updateUserLoginTime error: ' + err);
    }
  });
};

/*
* 快速注册一个玩家信息
* */
dbService.quickRegister = function(password, deviceID, registerType, callback) {
  var sql = 'insert into userinfo (password, registerType, registerDeviceID, registerTime) values (?, ?, ?, ?)';
  var args = [crypto.md5(password), registerType, deviceID, Date.now()];
  dataBase.query(sql, args, function(err, res) {
    if(err) {
      logger.error('registerUserinfo error: ' + err);
      callback(false);
      return;
    }
    callback(res.insertId);
    var sql1 = 'update userinfo set userName=? where id=?';
    var args1 =[res.insertId.toString(), res.insertId];
    dataBase.query(sql1, args1, function(err, res) {});
  });
};
