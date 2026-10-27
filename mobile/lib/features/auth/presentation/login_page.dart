import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/failure_messages.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';

/// Seeded test user of the backend TestOnlyDataSeeder (see mobile/README.md).
const _defaultMockUid = 'foundation-user-test-only';

class LoginPage extends ConsumerStatefulWidget {
  const LoginPage({super.key});

  @override
  ConsumerState<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<LoginPage> {
  // Mock mode (dev only) swaps email/password for a single uid field.
  late final bool _mock = ref.read(appConfigProvider).isMockAuth;
  late final FormGroup _form = _mock
      ? FormGroup({
          'uid': FormControl<String>(
            value: _defaultMockUid,
            validators: [
              Validators.required,
              Validators.pattern(r'^[A-Za-z0-9_-]{1,128}$'),
            ],
          ),
        })
      : FormGroup({
          'email': FormControl<String>(
            validators: [Validators.required, Validators.email],
          ),
          'password': FormControl<String>(validators: [Validators.required]),
        });
  Object? _error;
  bool _busy = false;

  Future<void> _submit() async {
    if (_busy) return;
    if (_form.invalid) {
      _form.markAllAsTouched();
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await ref
          .read(sessionProvider.notifier)
          .signIn(
            email: _mock
                ? _form.control('uid').value as String
                : _form.control('email').value as String,
            password: _mock ? '' : _form.control('password').value as String,
          );
    } on AuthFailure catch (e) {
      _error = e;
    } on ApiFailure catch (e) {
      _error = e;
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  void dispose() {
    _form.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final messages = <String, ValidationMessageFunction>{
      ValidationMessage.required: (_) => l10n.commonFieldRequired,
      ValidationMessage.email: (_) => l10n.errorValidation,
      ValidationMessage.pattern: (_) => l10n.errorValidation,
    };
    return ErpScreen(
      title: l10n.loginTitle,
      body: ListView(
        padding: ErpPagePadding.of(context),
        children: [
          if (_error != null) ...[
            ErpAlert(
              message: failureMessage(l10n, _error!),
              severity: ErpAlertSeverity.danger,
            ),
            SizedBox(height: t.space4),
          ],
          if (_mock) ...[
            ErpAlert(
              message: l10n.loginMockBanner,
              severity: ErpAlertSeverity.warning,
            ),
            SizedBox(height: t.space4),
            ErpTextField(
              control: _form.control('uid') as FormControl<String>,
              label: l10n.loginMockUidLabel,
              placeholder: l10n.loginMockUidPlaceholder,
              textInputAction: TextInputAction.done,
              validationMessages: messages,
            ),
          ] else ...[
            ErpTextField(
              control: _form.control('email') as FormControl<String>,
              label: l10n.loginEmailLabel,
              placeholder: l10n.loginEmailPlaceholder,
              keyboardType: TextInputType.emailAddress,
              textInputAction: TextInputAction.next,
              autofillHints: const [AutofillHints.username],
              validationMessages: messages,
            ),
            SizedBox(height: t.space4),
            ErpTextField(
              control: _form.control('password') as FormControl<String>,
              label: l10n.loginPasswordLabel,
              placeholder: l10n.loginPasswordPlaceholder,
              obscureText: true,
              textInputAction: TextInputAction.done,
              autofillHints: const [AutofillHints.password],
              validationMessages: messages,
            ),
          ],
          SizedBox(height: t.space6),
          ErpButton(
            label: l10n.loginSubmit,
            isLoading: _busy,
            onPressed: _submit,
          ),
        ],
      ),
    );
  }
}
