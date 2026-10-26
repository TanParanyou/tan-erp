import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/l10n/failure_messages.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_empty_state.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';

/// Biometric gate shown on every launch after the first sign-in.
class LockPage extends ConsumerStatefulWidget {
  const LockPage({super.key});

  @override
  ConsumerState<LockPage> createState() => _LockPageState();
}

class _LockPageState extends ConsumerState<LockPage> {
  Object? _error;
  bool _busy = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _unlock());
  }

  Future<void> _unlock() async {
    if (_busy) return;
    final reason = context.l10n.unlockReason;
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await ref.read(sessionProvider.notifier).unlock(reason);
    } on AuthFailure catch (e) {
      _error = e;
    } on ApiFailure catch (e) {
      _error = e;
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    return ErpScreen(
      body: Column(
        children: [
          Expanded(
            child: ErpEmptyState(
              icon: ErpIcons.user,
              title: l10n.unlockTitle,
              message: l10n.unlockBody,
            ),
          ),
          Padding(
            padding: EdgeInsets.all(t.space4),
            child: Column(
              children: [
                if (_error != null) ...[
                  ErpAlert(
                    message: failureMessage(l10n, _error!),
                    severity: ErpAlertSeverity.danger,
                  ),
                  SizedBox(height: t.space4),
                ],
                ErpButton(
                  label: l10n.unlockAction,
                  isLoading: _busy,
                  onPressed: _unlock,
                ),
                SizedBox(height: t.space2),
                ErpButton(
                  label: l10n.unlockUseOtherAccount,
                  variant: ErpButtonVariant.ghost,
                  onPressed: _busy
                      ? null
                      : () => ref.read(sessionProvider.notifier).signOut(),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}
