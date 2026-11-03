import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/auth/auth_failure.dart';
import 'package:tan_erp_field/core/auth/auth_repository.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/config/app_config.dart';
import 'package:tan_erp_field/core/l10n/failure_messages.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/network/api_failure.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_alert.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_card.dart';
import 'package:tan_erp_field/core/ui/erp_content_frame.dart';
import 'package:tan_erp_field/core/ui/erp_page_padding.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_text_field.dart';

const _defaultMockUid = 'foundation-user-test-only';

enum _LoginStep {
  credentials,
  twoFactorVerify,
  twoFactorSetup,
  backupCodesDisplay,
}

class LoginPage extends ConsumerStatefulWidget {
  const LoginPage({super.key});

  @override
  ConsumerState<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<LoginPage> {
  late final bool _mock = ref.read(appConfigProvider).isMockAuth;

  late final FormGroup _credentialsForm = _mock
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

  final FormGroup _twoFactorForm = FormGroup({
    'code': FormControl<String>(
      validators: [
        Validators.required,
        Validators.pattern(r'^[A-Za-z0-9]{6,16}$'),
      ],
    ),
  });

  final FormGroup _setupForm = FormGroup({
    'code': FormControl<String>(
      validators: [Validators.required, Validators.pattern(r'^\d{6}$')],
    ),
  });

  _LoginStep _step = _LoginStep.credentials;
  String _challengeToken = '';
  String _secretKey = '';
  List<String> _backupCodes = const <String>[];
  bool _copiedSecret = false;
  bool _copiedBackupCodes = false;
  Object? _error;
  bool _busy = false;

  Future<void> _submitCredentials() async {
    if (_busy) return;
    if (_credentialsForm.invalid) {
      _credentialsForm.markAllAsTouched();
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      final email = _mock
          ? _credentialsForm.control('uid').value as String
          : _credentialsForm.control('email').value as String;
      final password = _mock
          ? ''
          : _credentialsForm.control('password').value as String;

      final outcome = await ref
          .read(sessionProvider.notifier)
          .signIn(email: email, password: password);

      if (!mounted) return;

      switch (outcome) {
        case SignInCompleted():
          // Session state changes and router automatically redirects.
          break;
        case SignInTwoFactorRequired(:final challengeToken):
          setState(() {
            _challengeToken = challengeToken;
            _twoFactorForm.reset();
            _step = _LoginStep.twoFactorVerify;
          });
        case SignInTwoFactorSetupRequired(
          :final challengeToken,
          :final secretKey,
        ):
          setState(() {
            _challengeToken = challengeToken;
            _secretKey = secretKey;
            _copiedSecret = false;
            _setupForm.reset();
            _step = _LoginStep.twoFactorSetup;
          });
      }
    } on AuthFailure catch (e) {
      _error = e;
    } on ApiFailure catch (e) {
      _error = e;
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _submitTwoFactorVerify() async {
    if (_busy) return;
    if (_twoFactorForm.invalid) {
      _twoFactorForm.markAllAsTouched();
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      final code = _twoFactorForm.control('code').value as String;
      final backupCodes = await ref
          .read(sessionProvider.notifier)
          .verifyTwoFactor(challengeToken: _challengeToken, code: code);

      if (!mounted) return;

      if (backupCodes.isNotEmpty) {
        setState(() {
          _backupCodes = backupCodes;
          _copiedBackupCodes = false;
          _step = _LoginStep.backupCodesDisplay;
        });
      }
    } on AuthFailure catch (e) {
      _error = e;
    } on ApiFailure catch (e) {
      _error = e;
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _submitTwoFactorSetup() async {
    if (_busy) return;
    if (_setupForm.invalid) {
      _setupForm.markAllAsTouched();
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      final code = _setupForm.control('code').value as String;
      final backupCodes = await ref
          .read(sessionProvider.notifier)
          .verifyTwoFactor(challengeToken: _challengeToken, code: code);

      if (!mounted) return;

      if (backupCodes.isNotEmpty) {
        setState(() {
          _backupCodes = backupCodes;
          _copiedBackupCodes = false;
          _step = _LoginStep.backupCodesDisplay;
        });
      } else {
        await ref.read(sessionProvider.notifier).completeAuthenticatedSession();
      }
    } on AuthFailure catch (e) {
      _error = e;
    } on ApiFailure catch (e) {
      _error = e;
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _continueFromBackupCodes() async {
    if (_busy) return;
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await ref.read(sessionProvider.notifier).completeAuthenticatedSession();
    } on AuthFailure catch (e) {
      _error = e;
    } on ApiFailure catch (e) {
      _error = e;
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  void _backToCredentials() {
    setState(() {
      _step = _LoginStep.credentials;
      _error = null;
    });
  }

  @override
  void dispose() {
    _credentialsForm.dispose();
    _twoFactorForm.dispose();
    _setupForm.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;

    final title = switch (_step) {
      _LoginStep.credentials => l10n.loginTitle,
      _LoginStep.twoFactorVerify => l10n.loginTwoFactorTitle,
      _LoginStep.twoFactorSetup => l10n.loginTwoFactorSetupTitle,
      _LoginStep.backupCodesDisplay => l10n.loginBackupCodesTitle,
    };

    return ErpScreen(
      title: title,
      body: ErpContentFrame(
        child: ListView(
          padding: ErpPagePadding.of(context),
          children: [
            if (_error != null) ...[
              ErpAlert(
                message: failureMessage(l10n, _error!),
                severity: ErpAlertSeverity.danger,
              ),
              SizedBox(height: t.space4),
            ],
            switch (_step) {
              _LoginStep.credentials => _buildCredentialsView(context),
              _LoginStep.twoFactorVerify => _buildTwoFactorVerifyView(context),
              _LoginStep.twoFactorSetup => _buildTwoFactorSetupView(context),
              _LoginStep.backupCodesDisplay => _buildBackupCodesView(context),
            },
          ],
        ),
      ),
    );
  }

  Widget _buildCredentialsView(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final messages = <String, ValidationMessageFunction>{
      ValidationMessage.required: (_) => l10n.commonFieldRequired,
      ValidationMessage.email: (_) => l10n.errorValidation,
      ValidationMessage.pattern: (_) => l10n.errorValidation,
    };

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        if (_mock) ...[
          ErpAlert(
            message: l10n.loginMockBanner,
            severity: ErpAlertSeverity.warning,
          ),
          SizedBox(height: t.space4),
          ErpTextField(
            control: _credentialsForm.control('uid') as FormControl<String>,
            label: l10n.loginMockUidLabel,
            placeholder: l10n.loginMockUidPlaceholder,
            textInputAction: TextInputAction.done,
            validationMessages: messages,
          ),
        ] else ...[
          ErpTextField(
            control: _credentialsForm.control('email') as FormControl<String>,
            label: l10n.loginEmailLabel,
            placeholder: l10n.loginEmailPlaceholder,
            keyboardType: TextInputType.emailAddress,
            textInputAction: TextInputAction.next,
            autofillHints: const [AutofillHints.username],
            validationMessages: messages,
          ),
          SizedBox(height: t.space4),
          ErpTextField(
            control:
                _credentialsForm.control('password') as FormControl<String>,
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
          onPressed: _submitCredentials,
        ),
      ],
    );
  }

  Widget _buildTwoFactorVerifyView(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final messages = <String, ValidationMessageFunction>{
      ValidationMessage.required: (_) => l10n.commonFieldRequired,
      ValidationMessage.pattern: (_) => l10n.errorValidation,
    };

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          l10n.loginTwoFactorVerifyDesc,
          style: Theme.of(context).textTheme.bodyMedium
              ?.copyWith(color: t.textMuted),
        ),
        SizedBox(height: t.space4),
        ErpTextField(
          control: _twoFactorForm.control('code') as FormControl<String>,
          label: l10n.loginTwoFactorCodeLabel,
          placeholder: l10n.loginTwoFactorCodePlaceholder,
          keyboardType: TextInputType.text,
          textInputAction: TextInputAction.done,
          validationMessages: messages,
        ),
        SizedBox(height: t.space6),
        ErpButton(
          label: l10n.loginTwoFactorVerifyAction,
          isLoading: _busy,
          onPressed: _submitTwoFactorVerify,
        ),
        SizedBox(height: t.space3),
        ErpButton(
          label: l10n.loginTwoFactorBackToLogin,
          variant: ErpButtonVariant.outline,
          onPressed: _busy ? null : _backToCredentials,
        ),
      ],
    );
  }

  Widget _buildTwoFactorSetupView(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final textTheme = Theme.of(context).textTheme;
    final messages = <String, ValidationMessageFunction>{
      ValidationMessage.required: (_) => l10n.commonFieldRequired,
      ValidationMessage.pattern: (_) => l10n.errorValidation,
    };

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Text(
          l10n.loginTwoFactorSetupDesc,
          style: textTheme.bodyMedium?.copyWith(color: t.textMuted),
        ),
        SizedBox(height: t.space4),
        ErpCard(
          title: l10n.loginTwoFactorSecretKeyLabel,
          children: [
            SelectableText(
              _secretKey,
              style: textTheme.titleSmall?.copyWith(
                fontFamily: 'monospace',
                fontFeatures: const [FontFeature.tabularFigures()],
                color: t.textMain,
              ),
            ),
            SizedBox(height: t.space3),
            ErpButton(
              label: _copiedSecret
                  ? l10n.loginTwoFactorCopiedSecret
                  : l10n.loginTwoFactorCopySecret,
              variant: ErpButtonVariant.outline,
              onPressed: () async {
                await Clipboard.setData(ClipboardData(text: _secretKey));
                if (mounted) setState(() => _copiedSecret = true);
              },
            ),
          ],
        ),
        SizedBox(height: t.space4),
        ErpTextField(
          control: _setupForm.control('code') as FormControl<String>,
          label: l10n.loginTwoFactorSetupCodeLabel,
          placeholder: l10n.loginTwoFactorSetupCodePlaceholder,
          keyboardType: TextInputType.number,
          textInputAction: TextInputAction.done,
          validationMessages: messages,
        ),
        SizedBox(height: t.space6),
        ErpButton(
          label: l10n.loginTwoFactorCompleteSetupAction,
          isLoading: _busy,
          onPressed: _submitTwoFactorSetup,
        ),
        SizedBox(height: t.space3),
        ErpButton(
          label: l10n.loginTwoFactorBackToLogin,
          variant: ErpButtonVariant.outline,
          onPressed: _busy ? null : _backToCredentials,
        ),
      ],
    );
  }

  Widget _buildBackupCodesView(BuildContext context) {
    final l10n = context.l10n;
    final t = context.tokens;
    final textTheme = Theme.of(context).textTheme;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ErpAlert(
          message: l10n.loginBackupCodesWarning,
          severity: ErpAlertSeverity.warning,
        ),
        SizedBox(height: t.space4),
        ErpCard(
          title: l10n.loginBackupCodesTitle,
          children: [
            Wrap(
              spacing: t.space3,
              runSpacing: t.space2,
              children: [
                for (final code in _backupCodes)
                  DecoratedBox(
                    decoration: BoxDecoration(
                      color: t.surfaceSubtle,
                      borderRadius: t.corner,
                      border: Border.all(color: t.border),
                    ),
                    child: Padding(
                      padding: EdgeInsets.symmetric(
                        horizontal: t.space3,
                        vertical: t.space2,
                      ),
                      child: Text(
                        code,
                        style: textTheme.bodyMedium?.copyWith(
                          fontFamily: 'monospace',
                          fontFeatures: const [FontFeature.tabularFigures()],
                          color: t.textMain,
                        ),
                      ),
                    ),
                  ),
              ],
            ),
            SizedBox(height: t.space4),
            ErpButton(
              label: _copiedBackupCodes
                  ? l10n.loginBackupCodesCopied
                  : l10n.loginBackupCodesCopyAll,
              variant: ErpButtonVariant.outline,
              onPressed: () async {
                await Clipboard.setData(
                  ClipboardData(text: _backupCodes.join('\n')),
                );
                if (mounted) setState(() => _copiedBackupCodes = true);
              },
            ),
          ],
        ),
        SizedBox(height: t.space6),
        ErpButton(
          label: l10n.loginBackupCodesContinueAction,
          isLoading: _busy,
          onPressed: _continueFromBackupCodes,
        ),
      ],
    );
  }
}
