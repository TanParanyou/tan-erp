import 'package:analyzer/error/listener.dart';
import 'package:analyzer/error/error.dart' show DiagnosticSeverity;
import 'package:custom_lint_builder/custom_lint_builder.dart';

PluginBase createPlugin() => _ErpLints();

class _ErpLints extends PluginBase {
  @override
  List<LintRule> getLintRules(CustomLintConfigs configs) => [
    _BannedConstructorRule(
      code: const LintCode(
        name: 'no_hardcoded_color',
        problemMessage:
            'Do not build colours by hand; read them from ErpTokens (design.md 7.2).',
        errorSeverity: DiagnosticSeverity.ERROR,
      ),
      banned: const {'Color', 'Color.fromARGB', 'Color.fromRGBO', 'Color.from'},
    ),
    _BannedConstructorRule(
      code: const LintCode(
        name: 'no_hardcoded_border_radius',
        problemMessage:
            'Do not hard-code corner radius; use the radius token (design.md 7.2).',
        errorSeverity: DiagnosticSeverity.ERROR,
      ),
      banned: const {
        'BorderRadius.circular',
        'Radius.circular',
        'BorderRadius.all',
      },
    ),
  ];
}

/// Flags the banned constructor calls anywhere outside lib/core/theme/.
class _BannedConstructorRule extends DartLintRule {
  const _BannedConstructorRule({required super.code, required this.banned});

  final Set<String> banned;

  @override
  void run(
    CustomLintResolver resolver,
    DiagnosticReporter reporter,
    CustomLintContext context,
  ) {
    if (resolver.path.contains('/lib/core/theme/')) return;
    context.registry.addInstanceCreationExpression((node) {
      if (banned.contains(node.constructorName.toString())) {
        reporter.atNode(node, code);
      }
    });
  }
}
