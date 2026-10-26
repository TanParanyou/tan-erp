import 'package:flutter/material.dart';
import 'package:reactive_forms/reactive_forms.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Central text input bound to a reactive_forms [FormControl]. The label sits
/// above the field; [placeholder] must come from l10n.
class ErpTextField extends StatelessWidget {
  const ErpTextField({
    required this.control,
    required this.label,
    required this.placeholder,
    this.validationMessages,
    this.obscureText = false,
    this.keyboardType,
    this.textInputAction,
    this.autofillHints,
    super.key,
  });

  final FormControl<String> control;
  final String label;
  final String placeholder;
  final Map<String, ValidationMessageFunction>? validationMessages;
  final bool obscureText;
  final TextInputType? keyboardType;
  final TextInputAction? textInputAction;
  final Iterable<String>? autofillHints;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(label, style: Theme.of(context).textTheme.labelLarge),
        SizedBox(height: t.space1),
        ConstrainedBox(
          constraints: BoxConstraints(minHeight: t.tapTarget),
          child: ReactiveTextField<String>(
            formControl: control,
            obscureText: obscureText,
            keyboardType: keyboardType,
            textInputAction: textInputAction,
            autofillHints: autofillHints,
            validationMessages: validationMessages,
            decoration: InputDecoration(hintText: placeholder),
          ),
        ),
      ],
    );
  }
}
