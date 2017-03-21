/**
 * 
 * 常量定义
 */
module.exports = {
  // 游戏模式定义
  MODE: {
    FREE:               "free",               //免费模式
    CHARGE:             "charge"              //付费模式
  },

  // 座位位置定义
  CHAIR: {
    LEFT:               "left",               //左边座位
    RIGHT:              "right"               //右边座位
  },

  // 定时器时间(毫秒)
  INTERVAL: {
    ADD_FISH:           1000,                 //出鱼时间间隔
    SYNC_TIME:          100,                  //服务器与客户端时间同步间隔
    CLEAR_FISH:         2000                  //清理过期鱼的时间间隔
  },

  MAX_FISH_NUM:         300,                  //同屏最大鱼群数量

  // 错误类型
  ERR: {
    LOGIC_ERR:          "logic_error",        //逻辑错误(客户端请求参数、服务器逻辑等)
    DB_ERR:             "db_error"            //数据库错误
  },

  // 鱼钩类型
  HOOK:                 [1, 2, 3],            //1、2、3个鱼钩

  // 分值、金币兑换比例
  TRANS_RATE:           10
};