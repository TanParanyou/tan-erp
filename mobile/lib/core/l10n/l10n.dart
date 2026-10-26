import 'package:flutter/widgets.dart';
import 'package:tan_erp_field/core/l10n/gen/app_localizations.dart';

export 'package:tan_erp_field/core/l10n/gen/app_localizations.dart';

extension L10nContext on BuildContext {
  AppLocalizations get l10n => AppLocalizations.of(this);
}
