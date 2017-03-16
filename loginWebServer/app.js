/**
 * 
 * 登录认证服务器主文件
 */
var express = require('express');
var http = require('http');
var path = require('path');
var CONSTDEFINE = require('./const/constDefine.js');
// 引入路由定义文件
var localLogin = require('./routes/localLogin.js');

var app = express();

// 环境设置
app.set('port', process.env.PORT || 3000);
app.set('views', __dirname + '/views');
app.set('view engine', 'ejs');
app.use(express.favicon());
app.use(express.logger('dev'));
app.use(express.bodyParser());
app.use(express.methodOverride());
app.use(app.router);
app.use(express.static(path.join(__dirname, 'public')));

// 开发环境设置
if ('development' == app.get('env')) {
  app.use(express.errorHandler());
}

/*
*  为不同的请求路径分发不同路由，具体的不同渠道登录、注册，都在这里分发到具体逻辑
* */
app.post('/login', localLogin.login);                 //本地登录
app.post('/register', localLogin.register);           //本地注册
app.post('/quickRegister', localLogin.quickRegister); //快速登录

//设置监听端口
app.listen(CONSTDEFINE.LISTEN_PORT);
console.log('登录认证服务器启动成功');