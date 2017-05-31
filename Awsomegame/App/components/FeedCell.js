'use strict'

import React, { Component } from 'react';
import {
  Image,
  StyleSheet,
  Text,
  ScrollView,
  TouchableOpacity,
  View,
  Dimensions
} from 'react-native';

import URLConf from '../api/URLConf';
import { formatDate } from '../utils/DateUtil';
import CommentList from './CommentList';

const windowWidth = Dimensions.get('window').width;
const margin = 20;
const imgInterval = 5;

const img_thumbnail = '?imageView2/1/w/200/h/200';
const avatar_thumbnail = '?imageView2/1/w/100/h/100';

export default class FeedCell extends Component {
  renderFeedImages = (content) => {
    if (content == null) return [];
    var images = content.split(":");
    var imagesView = [];
    for (var i = 0; i < images.length - 1; i++) {
      imagesView.push(
        <Image source={{ uri: URLConf.IMG_BASE_URL + images[i] + img_thumbnail }} style={styles.feedContentImage} />
      )
      return imagesView;
    }
  };

  renderCommentList() {
    return (
      <View style={{ flex: 1 }}>
        <View style={{ flex: 1, flexDirection: 'row' }}>
          {this.props.feed.tags && <ScrollView horizontal={true} showsHorizontalScrollIndicator={false} style={styles.tagsContainer}>
            {
              this.props.feed.tags.map(tag => <TouchableOpacity
                onPress={() => this.props.nav2TagDetail(tag)}>
                <Text style={{ fontSize: 16, color: '#9B9B9B', marginRight: 5 }}>{tag.tag}</Text>
              </TouchableOpacity>)

            }
          </ScrollView>
          }
          <FeedActions
            feed={this.props.feed}
            likeCounter={this.props.feed.like_count}
            commentCounter={this.props.feed.comment_count}
            push2FeedDetail={this.props.push2FeedDetail}
            refresh={this.props.refresh} />
        </View>
        <CommentList
          secret={this.props.secret}
          token={this.props.token}
          object_type={this.props.feed.object_type}
          object_id={this.props.feed.object_id}
          liked={false}
          commented={false}
          commentCounter={this.props.feed.comment_count}
          push2FeedDetail={this.props.push2FeedDetail}
          limit={5}
          refresh={this.props.refresh}
        />
      </View>
    );
  };

  nav2TagDetail(tag) {
    console.log(tag.tag);
    this.props.navigator.push({
      title: tag.tag,
      component: TagDetail,
      params: { token: this.props.token }
    });
  };

  renderFeedContent = (feed) => {
    if (feed.summary == null || feed.summary.length == 0) {
      return (
        <View style={styles.feedContentImage}>{this.renderFeedImages(this.props.feed.content)}</View>
      );
    }
    return (
      <View>
        <Text style={styles.feedContentText}>{this.props.feed.summary}</Text>
        <View style={styles.feedContentImage}>{this.renderFeedImages(this.props.feed.content)}</View>
      </View>
    );
  };

  componentWillMount() {
    console.log(this.props.newComment);
  };

  render() {
    return (
      <View>
        <TouchableOpacity
          onPress={this.props.onSelect}>
          <View style={styles.container}>
            <View style={styles.feedHeader}>
              <View>
                <TouchableOpacity onPress={this.props.pressAvatar}>
                  <Image source={require('../../assets/img/default-avatar.jpg')/*{ uri: URLConf.IMG_BASE_URL + this.props.feed.user_avatar + avatar_thumbnail }*/} style={styles.avatar} />
                </TouchableOpacity>
              </View>
              <View style={styles.feedUserInfo}>
                <Text style={styles.feedUserName}>{"test"/*this.props.feed.user_name*/}</Text>
                <Text style={styles.feedTime}>{"刚刚"/*formatDate(this.props.feed.ts)*/}</Text>
                {/* <Text style={styles.feedTime}>{this.props.feed.id+' '+this.props.page}</Text> */}
              </View>
            </View>
            <View style={styles.feedContent}>
              {this.renderFeedContent(this.props.feed)}
            </View>
          </View>
        </TouchableOpacity>
      </View>
    );
  }
}


var styles = StyleSheet.create({
  container: {
    flex: 1,
    flexDirection: 'column',
    marginBottom: 0,
    paddingBottom: 10,
    backgroundColor: 'white',
    //borderTopWidth: 0.5,
    borderBottomWidth: 0.5,
    borderColor: '#EEEEEE',
  },
  feedHeader: {
    flex: 1,
    flexDirection: 'row',
    margin: margin,
  },
  avatar: {
    width: 40,
    height: 40,
    borderRadius: 20,
  },
  feedUserInfo: {
    marginLeft: 10,
  },

  feedUserName: {
    marginTop: 3,
    fontSize: 16,
    color: '#00B5AD',
    lineHeight: 16,
  },
  feedTime: {
    fontSize: 12,
    color: '#7B7C84',
    lineHeight: 12,
    marginTop: 5,
  },

  feedContent: {
    flex: 1,
  },
  feedContentText: {
    flex: 1,
    textAlign: 'justify',
    margin: margin,
    marginTop: -10,
    fontSize: 16,
    color: '#333333',
    lineHeight: 20,
  },
  feedContentSingleImage: {
    flex: 1,
    height: 170,
  },
  feedContentImages: {
    flex: 1,
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginLeft: margin,
  },
  feedContentImage: {
    width: (windowWidth - margin * 2 - imgInterval * 2) / 3,
    height: (windowWidth - margin * 2 - imgInterval * 2) / 3,
    marginBottom: imgInterval,
    marginRight: imgInterval,
  },
  feedActions: {
    //borderWidth: 1,
    //borderTopColor: '#EEEEEE',
    flex: 1,
    flexDirection: 'row',
    //marginTop: 15,
    marginRight: margin,
    marginBottom: 5,
  },
  feedActionComment: {
    width: 40,
    padding: 5,
    marginRight: 5,
  },
  feedActionLike: {
    width: 40,
    padding: 5,
  },
  thumbnail: {
    flex: 1,
    height: 81,
  },
  rightContainer: {
    flex: 1,
  },
  listView: {
    paddingTop: 70,
    backgroundColor: 'white',
  },
  tagsContainer: {
    flex: 2,
    marginLeft: 20,
    marginTop: 10,

  }
});