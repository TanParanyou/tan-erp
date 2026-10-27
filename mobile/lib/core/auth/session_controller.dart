import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/auth/biometric_gate.dart';
import 'package:tan_erp_field/core/auth/current_user.dart';
import 'package:tan_erp_field/core/auth/session_storage.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/locale_provider.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/theme/theme_settings.dart';

@immutable
sealed class Session {
  const Session();
}

/// Firebase is not configured for this flavor.
class SessionUnavailable extends Session {
  const SessionUnavailable();
}

class SessionSignedOut extends Session {
  const SessionSignedOut();
}

/// A persisted user exists; the biometric gate must pass first.
class SessionLocked extends Session {
  const SessionLocked();
}

class SessionSelectMembership extends Session {
  const SessionSelectMembership(this.user);

  final CurrentUser user;
}

class SessionActive extends Session {
  const SessionActive(this.user, this.membership);

  final CurrentUser user;
  final Membership membership;
}

class SessionController extends Notifier<Session> {
  @override
  Session build() {
    final config = ref.watch(appConfigProvider);
    if (!config.firebase.isConfigured && !config.isMockAuth) {
      return const SessionUnavailable();
    }
    return ref.read(authRepositoryProvider).hasSession
        ? const SessionLocked()
        : const SessionSignedOut();
  }

  /// Throws [AuthFailure] or [ApiFailure]; the session stays signed out then.
  Future<void> signIn({required String email, required String password}) async {
    await ref
        .read(authRepositoryProvider)
        .signIn(email: email, password: password);
    await _loadOrSignOut();
  }

  /// Biometric gate on reopening the app, then the current user is loaded.
  Future<void> unlock(String reason) async {
    final gate = ref.read(biometricGateProvider);
    if (await gate.isAvailable() && !await gate.authenticate(reason)) {
      throw const BiometricFailedFailure();
    }
    try {
      await _load();
    } on NetworkFailure {
      rethrow; // stay locked so the user can retry offline-to-online
    } on ApiFailure {
      await signOut();
      rethrow;
    }
  }

  Future<void> selectMembership(Membership membership) async {
    final current = state;
    if (current is SessionSelectMembership) {
      await ref.read(sessionStorageProvider).writeMembershipId(membership.id);
      state = SessionActive(current.user, membership);
    }
  }

  void switchBranch() {
    final current = state;
    if (current is SessionActive) state = SessionSelectMembership(current.user);
  }

  Future<void> signOut() async {
    await ref.read(authRepositoryProvider).signOut();
    await ref.read(sessionStorageProvider).clear();
    state = const SessionSignedOut();
  }

  Future<void> _loadOrSignOut() async {
    try {
      await _load();
    } on ApiFailure {
      await signOut();
      rethrow;
    }
  }

  Future<void> _load() async {
    final user = await ref.read(currentUserRepositoryProvider).load();
    if (user.memberships.isEmpty) {
      throw const ForbiddenFailure(code: 'ACTIVE_MEMBERSHIP_REQUIRED');
    }
    // Server preferences win on every login/unlock; null keeps local values.
    final preferences = user.preferences;
    if (preferences != null) {
      ref.read(themeSettingsProvider.notifier).settings = preferences;
    }
    final locale = user.preferredLocale;
    if (locale != null) ref.read(localeProvider.notifier).locale = locale;
    final lastId = await ref.read(sessionStorageProvider).readMembershipId();
    final remembered = user.memberships.where((m) => m.id == lastId);
    if (remembered.isNotEmpty) {
      state = SessionActive(user, remembered.first);
    } else if (user.memberships.length == 1) {
      final only = user.memberships.first;
      await ref.read(sessionStorageProvider).writeMembershipId(only.id);
      state = SessionActive(user, only);
    } else {
      state = SessionSelectMembership(user);
    }
  }
}

final sessionProvider = NotifierProvider<SessionController, Session>(
  SessionController.new,
);
