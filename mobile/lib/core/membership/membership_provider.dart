import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';

/// Membership sent as `X-Membership-Id`; null until one is chosen.
final activeMembershipIdProvider = Provider<String?>((ref) {
  final session = ref.watch(sessionProvider);
  return session is SessionActive ? session.membership.id : null;
});
