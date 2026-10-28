import 'dart:async';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

enum PushPermissionStatus {
  unavailable,
  disabled,
  enabled,
}

@immutable
class PushNotificationState {
  const PushNotificationState({
    required this.status,
    this.fcmToken,
    this.isRequesting = false,
  });

  final PushPermissionStatus status;
  final String? fcmToken;
  final bool isRequesting;

  PushNotificationState copyWith({
    PushPermissionStatus? status,
    String? fcmToken,
    bool? isRequesting,
  }) {
    return PushNotificationState(
      status: status ?? this.status,
      fcmToken: fcmToken ?? this.fcmToken,
      isRequesting: isRequesting ?? this.isRequesting,
    );
  }
}

class PushNotificationController extends Notifier<PushNotificationState> {
  @override
  PushNotificationState build() {
    unawaited(Future<void>.microtask(_checkInitialStatus));
    return const PushNotificationState(
      status: PushPermissionStatus.disabled,
    );
  }

  Future<void> _checkInitialStatus() async {
    try {
      final messaging = FirebaseMessaging.instance;
      final settings = await messaging.getNotificationSettings();
      final isAuthorized =
          settings.authorizationStatus == AuthorizationStatus.authorized ||
              settings.authorizationStatus == AuthorizationStatus.provisional;
      if (isAuthorized) {
        // FCM getToken is still the cross-platform token accessor in v16.
        // ignore: deprecated_member_use
        final token = await messaging.getToken();
        state = state.copyWith(
          status: PushPermissionStatus.enabled,
          fcmToken: token,
        );
      } else {
        state = state.copyWith(status: PushPermissionStatus.disabled);
      }
    } on Object {
      state = state.copyWith(status: PushPermissionStatus.unavailable);
    }
  }

  Future<void> requestPermissionAndToken() async {
    state = state.copyWith(isRequesting: true);
    try {
      final messaging = FirebaseMessaging.instance;
      final settings = await messaging.requestPermission();
      final isAuthorized =
          settings.authorizationStatus == AuthorizationStatus.authorized ||
              settings.authorizationStatus == AuthorizationStatus.provisional;
      if (isAuthorized) {
        // FCM getToken is still the cross-platform token accessor in v16.
        // ignore: deprecated_member_use
        final token = await messaging.getToken();
        state = state.copyWith(
          status: PushPermissionStatus.enabled,
          fcmToken: token,
          isRequesting: false,
        );
      } else {
        state = state.copyWith(
          status: PushPermissionStatus.disabled,
          isRequesting: false,
        );
      }
    } on Object {
      state = state.copyWith(
        status: PushPermissionStatus.unavailable,
        isRequesting: false,
      );
    }
  }
}

final pushNotificationProvider =
    NotifierProvider<PushNotificationController, PushNotificationState>(
  PushNotificationController.new,
);
