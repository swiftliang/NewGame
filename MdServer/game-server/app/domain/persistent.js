
var EventEmitter = require('events').EventEmitter;
var util = require('util');

var Persistent = function(opts) {
    EventEmitter.call(this);
};

util.inherits(Persistent, EventEmitter);

module.exports = Persistent;

Persistent.prototype.Coin = function() {
    this.emit('coin');
};

Persistent.prototype.Level = function() {
    this.emit('level');
};