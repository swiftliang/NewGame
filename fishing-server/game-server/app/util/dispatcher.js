var crc = require('crc');

// 自带的负载均衡策略
module.exports.dispatch = function(uid, connectors) {
   var index = Math.abs(crc.crc32(uid)) % connectors.length;
   return connectors[index];
};

// 根据计数器做均衡策略(粗糙)，只是均衡分配到各connector，没有根据connector当前负载情况分配
module.exports.dispatchByCount = function(count, connectors) {
   //var index = count % connectors.length;
   var index = 0;
   return connectors[index];
};