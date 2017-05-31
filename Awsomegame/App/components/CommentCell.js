'use strict';

import React, {Component} from 'react';

import {
  ListView,
  Platform,
  StyleSheet,
  Text,
  Image,
  View,
  Modal,
  TouchableOpacity,
  TouchableHighlight,
  TouchableNativeFeedback,
} from 'react-native';

import URLConf from '../api/URLConf';
import {getToken} from '../utils/Secret';
import Md5 from '../utils/Md5';

const avatar_thumbnail = '?imageView2/1/w/48/h/48';

export default class CommentCell extends Component{
    constructor(props){
        super(props);
        this.setState({
            loginRegPageVisible: false
        });
    }
    render(){
        return (
            <View>
                //{this.state.loginRegPageVisible && <PopupLoginRegPage hideLoginRegPage={this.hideLoginRegPage} refresh={this.refresh} />}
                <TouchableOpacity onPress={this.onPress}>
                    <View style={styles.commentBox}>
                        <Image style={styles.avatar} source={{ uri: URLConf.IMG_BASE_URL + this.props.comment.comment_author_avatar + avatar_thumbnail }} />
                        <View>
                            {this.renderAuthorName(this.props.comment)}
                            <Text style={styles.comment}>{this.props.comment.comment_content}</Text>
                        </View>
                    </View>
                </TouchableOpacity>
            </View>
        );
    }
}

var styles = StyleSheet.create({
    commentBox: {
        flex: 1,
        flexDirection: 'row',
        //borderColor: 'black',
        //borderWidth: 1,
        padding: 10,
    },
    avatar: {
        borderRadius: 16,
        width: 32,
        height: 32,
        marginRight: 10,
    },
    username: {
        fontSize: 12,
        color: '#00B5AD',
        lineHeight: 13,
        marginBottom: 5,
    },
    commentTime: {

    },
    comment: {
        fontSize: 14,
        marginRight: 30,
        color: '#030303',
        lineHeight: 18,
    },
});
