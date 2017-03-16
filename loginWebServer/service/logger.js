/**
 * 
 * 日志记录模块
 */
var log4js = require('log4js');
var fs = require('fs');

// 对外暴露logger对象，logger在后面定义
var logger = {};
exports.logger = logger;

// 常量定义
var LOG_DIR = './logs';//日志文件存放的路径
var LOG_CONFIG = {      //log4js的配置
  appenders: [
    {
      type: "console"
    },
    {
      type: "dateFile",
      filename: "./logs/",
      pattern: "yyyy-MM-dd.txt",
      absolute: true,
      alwaysIncludePattern: true,
      category: "logger"
    }
  ],
  replaceConsole: true,
  levels: {
    "console": "DEBUG",
    "logger": "DEBUG"
  }
};

// 如果日志文件不存在，则新建一个
if (!fs.existsSync(LOG_DIR)){
  fs.mkdirSync(LOG_DIR);
}

// 目录创建完毕，加载配置
log4js.configure(LOG_CONFIG);

var log = log4js.getLogger('logger');

/*
 *  对外暴露log4js的六种级别的日志记录(级别由低到高)
 * */
// trace
logger.trace = function(msg) {
  if(msg == null) {
    msg = "";
  }
  log.trace(msg);
};

// debug
logger.debug = function(msg) {
  if(msg == null) {
    msg = "";
  }
  log.debug(msg);
};

// info
logger.info = function(msg) {
  if(msg == null) {
    msg = "";
  }
  log.info(msg);
};

// warn
logger.warn = function(msg) {
  if(msg == null) {
    msg = "";
  }
  log.warn(msg);
};

// error
logger.error = function(msg) {
  if(msg == null) {
    msg = "";
  }
  log.error(msg);
};

// fatal
logger.fatal = function(msg) {
  if(msg == null) {
    msg = "";
  }
  log.fatal(msg);
};

//exports.use = function(app) {
//  //页面请求日志, level用auto时,默认级别是WARN
//  app.use(log4js.connectLogger(log, {level:'debug', format:':method :url'}));
//};
