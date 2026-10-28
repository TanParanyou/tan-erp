import 'package:flutter/material.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_icon.dart';
import 'package:tan_erp_field/core/ui/erp_sync_banner.dart';

class ErpNavDestination {
  const ErpNavDestination({required this.icon, required this.label});

  final ErpIcons icon;
  final String label;
}

/// Bottom navigation on phones, navigation rail at >= 768dp (design.md 8.4).
class ErpNavShell extends StatelessWidget {
  const ErpNavShell({
    required this.destinations,
    required this.selectedIndex,
    required this.onSelected,
    required this.child,
    super.key,
  });

  static const double railBreakpoint = 768;

  final List<ErpNavDestination> destinations;
  final int selectedIndex;
  final ValueChanged<int> onSelected;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    final t = context.tokens;
    final wide = MediaQuery.sizeOf(context).width >= railBreakpoint;

    Widget iconFor(ErpNavDestination d, {required bool selected}) =>
        ErpIcon(d.icon, color: selected ? t.action : t.textMuted);

    final bodyWithBanner = Column(
      children: [
        const ErpSyncBanner(),
        Expanded(child: child),
      ],
    );

    if (wide) {
      return Scaffold(
        body: SafeArea(
          child: Row(
            children: [
              NavigationRail(
                selectedIndex: selectedIndex,
                onDestinationSelected: onSelected,
                labelType: NavigationRailLabelType.all,
                destinations: [
                  for (var i = 0; i < destinations.length; i++)
                    NavigationRailDestination(
                      icon: iconFor(destinations[i], selected: false),
                      selectedIcon: iconFor(destinations[i], selected: true),
                      label: Text(destinations[i].label),
                    ),
                ],
              ),
              VerticalDivider(width: t.borderWidth, color: t.border),
              Expanded(child: bodyWithBanner),
            ],
          ),
        ),
      );
    }
    return Scaffold(
      body: SafeArea(child: bodyWithBanner),
      bottomNavigationBar: NavigationBar(
        selectedIndex: selectedIndex,
        onDestinationSelected: onSelected,
        destinations: [
          for (final d in destinations)
            NavigationDestination(
              icon: iconFor(d, selected: false),
              selectedIcon: iconFor(d, selected: true),
              label: d.label,
            ),
        ],
      ),
    );
  }
}
