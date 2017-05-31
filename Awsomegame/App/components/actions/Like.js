import React from 'react';

import {
    View,
    Image,
    Text,
    TouchableOpacity,
    TouchableHighlight
} from 'react-native';

import { getToken } from '../../utils/Secret';
import { like, undoLike } from '../../api/ActionAPI';
import PopupLoginRegPage from '../../PopupLoginRegPage';

export default class Like extends React.Component {

    state = {
        counter: this.props.counter,
        isLiked: this.props.feed.is_like,
        loginRegPageVisible: false,
    };

    pressLike() {
        //getToken(this.checkLogin);

        if (this.state.isLiked) {
            undoLike(this.props.feed.object_type, this.props.feed.object_id, (result, err) => {
                if (!result) {
                    if (err == 'not logged in') {
                        this.setState({ loginRegPageVisible: true });
                    }
                } else {
                    this.setState({
                        isLiked: !this.state.isLiked,
                        counter: this.state.counter - 1,
                    });
                }
            });
        } else {
            like(this.props.feed.user_id, this.props.feed.object_type, this.props.feed.object_id, (result, err) => {
                if (!result) {
                    if (err == 'not logged in') {
                        this.setState({ loginRegPageVisible: true });
                    }
                } else {
                    this.setState({
                        isLiked: !this.state.isLiked,
                        counter: this.state.counter + 1,
                    });
                }
            });
        }
    };

    hideLoginRegPage() {
        this.setState({
            loginRegPageVisible: false,
        });
    };

    refresh(isLogin, token) {
        this.setState({
            loginRegPageVisible: false,
        }, this.props.refresh(isLogin, token));
    };

    render() {
        return (
            <View style={{ flexDirection: 'row', }}>
                {this.state.loginRegPageVisible && <PopupLoginRegPage hideLoginRegPage={this.hideLoginRegPage} refresh={this.refresh} />}
                <TouchableOpacity onPress={this.pressLike}>
                    {this.state.isLiked ?
                        <Image style={{ width: 22, height: 22, marginRight: 5 }} source={require('../../../assets/img/like.png')} /> :
                        <Image style={{ width: 22, height: 22, marginRight: 5 }} source={require('../../../assets/img/like_empty.png')} />
                    }

                </TouchableOpacity>
                <Text style={{ color: '#adadad' }}>{this.state.counter}</Text>
            </View>
        );
    }
}
