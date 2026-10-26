import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Minimal single-colour loading indicator (no skeletons).
class ErpMonoLoading extends StatelessWidget {
  const ErpMonoLoading({this.size = 24, this.color, super.key});

  final double size;
  final Color? color;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    return Center(
      child: SizedBox.square(
        dimension: size,
        child: CircularProgressIndicator(
          strokeWidth: t.borderWidth * 2,
          color: color ?? t.textMuted,
        ),
      ),
    );
  }
}
