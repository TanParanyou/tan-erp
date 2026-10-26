import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';

/// Confirmation for risky actions. The confirm button shows loading and the
/// dialog cannot be dismissed while [onConfirm] runs. Returns true on confirm.
Future<bool> showErpConfirmDialog(
  BuildContext context, {
  required String title,
  required String message,
  required String confirmLabel,
  required String cancelLabel,
  required Future<void> Function() onConfirm,
}) async {
  final result = await showDialog<bool>(
    context: context,
    barrierDismissible: false,
    builder: (_) => _ErpConfirmDialog(
      title: title,
      message: message,
      confirmLabel: confirmLabel,
      cancelLabel: cancelLabel,
      onConfirm: onConfirm,
    ),
  );
  return result ?? false;
}

class _ErpConfirmDialog extends StatefulWidget {
  const _ErpConfirmDialog({
    required this.title,
    required this.message,
    required this.confirmLabel,
    required this.cancelLabel,
    required this.onConfirm,
  });

  final String title;
  final String message;
  final String confirmLabel;
  final String cancelLabel;
  final Future<void> Function() onConfirm;

  @override
  State<_ErpConfirmDialog> createState() => _ErpConfirmDialogState();
}

class _ErpConfirmDialogState extends State<_ErpConfirmDialog> {
  bool _busy = false;

  Future<void> _confirm() async {
    setState(() => _busy = true);
    try {
      await widget.onConfirm();
    } finally {
      if (mounted) Navigator.of(context).pop(true);
    }
  }

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    return AlertDialog(
      title: Text(widget.title),
      content: Text(widget.message),
      actionsPadding: EdgeInsets.all(t.space4),
      actions: [
        ErpButton(
          label: widget.cancelLabel,
          variant: ErpButtonVariant.outline,
          expand: false,
          onPressed: _busy ? null : () => Navigator.of(context).pop(false),
        ),
        ErpButton(
          label: widget.confirmLabel,
          expand: false,
          isLoading: _busy,
          onPressed: _confirm,
        ),
      ],
    );
  }
}
