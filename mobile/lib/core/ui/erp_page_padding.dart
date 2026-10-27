import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';

/// Standard page-body inset (space4 on all sides) as a value, so scrollables
/// keep their own padding: `ListView(padding: ErpPagePadding.of(context))`.
class ErpPagePadding {
  const ErpPagePadding._();

  static EdgeInsets of(BuildContext context) =>
      EdgeInsets.all(context.tokens.space4);
}
