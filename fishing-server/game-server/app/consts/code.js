/**
 * 
 * 与客户端通信状态码常量定义
 */
module.exports = {
  SUCCESS:                200,            //成功
  FAILED:                 500,            //失败(服务器内部出错)
  DATA_ERROR:             404,            //未查询到(请求数据错误)
  SITTED:                 1001,           //某个座位已经被坐
  LACK_SCORE:             1002            //分值不足
};