/**
 * 
 * 通信路由常量的定义
 */
module.exports = {
  ADD_FISH:             "onAddFish",                    //出鱼
  FISH_STATUS:          "onFishStatus",                 //桌子上出鱼的状态
  SYNC_TIME:            "onSyncTime",                   //服务器与客户端时间同步
  SHOOT:                "onShoot",                      //客户端出钩事件
  TOUCH_FISH:           "onTouchFish",                  //钩子碰撞到鱼事件
  FISH_RESULT:          "onFishResult",                 //钓鱼结果通知事件
  NEW_ENTER:            "onNewEnter",                   //桌子上有新玩家加入
  PLAYER_QUIT:          "onPlayerQuit",                 //桌子上有人离开
  CHANGE_HOOK:          "onChangeHook",                 //切换鱼钩
  CHAT:                 "onChat"                        //聊天
};