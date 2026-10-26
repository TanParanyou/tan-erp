import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:tan_erp_field/core/auth/current_user.dart';
import 'package:tan_erp_field/core/auth/session_controller.dart';
import 'package:tan_erp_field/core/l10n/l10n.dart';
import 'package:tan_erp_field/core/theme/erp_tokens.dart';
import 'package:tan_erp_field/core/ui/erp_button.dart';
import 'package:tan_erp_field/core/ui/erp_screen.dart';
import 'package:tan_erp_field/core/ui/erp_select.dart';

class MembershipPage extends ConsumerStatefulWidget {
  const MembershipPage({super.key});

  @override
  ConsumerState<MembershipPage> createState() => _MembershipPageState();
}

class _MembershipPageState extends ConsumerState<MembershipPage> {
  Membership? _selected;
  bool _busy = false;

  Future<void> _continue(Membership m) async {
    setState(() => _busy = true);
    try {
      await ref.read(sessionProvider.notifier).selectMembership(m);
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final session = ref.watch(sessionProvider);
    if (session is! SessionSelectMembership) return const SizedBox.shrink();
    final l10n = context.l10n;
    final t = context.tokens;
    final selected = _selected;
    return ErpScreen(
      title: l10n.membershipTitle,
      body: Column(
        children: [
          Expanded(
            child: ListView(
              padding: EdgeInsets.all(t.space4),
              children: [
                ErpSelect<Membership>(
                  label: l10n.membershipLabel,
                  placeholder: l10n.commonSelect,
                  value: selected,
                  options: [
                    for (final m in session.user.memberships)
                      ErpOption(
                        value: m,
                        label: m.branchName ?? l10n.membershipAllBranches,
                        subtitle: m.organizationName,
                      ),
                  ],
                  onChanged: (m) => setState(() => _selected = m),
                ),
              ],
            ),
          ),
          Padding(
            padding: EdgeInsets.all(t.space4),
            child: ErpButton(
              label: l10n.membershipContinue,
              isLoading: _busy,
              onPressed: selected == null ? null : () => _continue(selected),
            ),
          ),
        ],
      ),
    );
  }
}
