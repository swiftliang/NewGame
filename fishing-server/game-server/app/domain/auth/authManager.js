/**
 * 
 * 登录认证逻辑处理模块
 */
var userDao = require('../../dao/userDao.js'),
    CODE = require('../../consts/code.js');

var authManager = module.exports;

/*
* 客户端使用username、password登录时，做认证
* */
authManager.checkLogin = function(username, password, cb) {
  userDao.getPlayerByUserName(username, password, function(err, playerList) {
    if(err || !playerList.length) {
      return cb(null, CODE.DATA_ERROR, null);
    }

    cb(null, CODE.SUCCESS, playerList[0]);
  });
};