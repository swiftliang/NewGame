/**
 * 
 * 玩家信息相关读取操作的处理模块
 */
var pomelo = require('pomelo');

var userDao = module.exports;

/*
* 根据username， password查找玩家信息
* */
userDao.getPlayerByUserName = function(username, password, cb) {
  var sql = "select * from user where username=? and password=?";
  var args = [username, password];

  pomelo.app.get('dbClient').query(sql, args, function(err, res) {
    console.log('--------------------');
    console.log(err, res);
    if(err) {
      return cb(err, null);
    }

    return cb(null, JSON.parse(JSON.stringify(res)));
  });
};