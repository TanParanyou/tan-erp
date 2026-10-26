// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_definition_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleDefinitionSummaryResponse extends RoleDefinitionSummaryResponse {
  @override
  final String? id;
  @override
  final String? name;
  @override
  final String? description;
  @override
  final bool? isSystem;
  @override
  final bool? isActive;
  @override
  final String? rowVersion;
  @override
  final int? memberCount;
  @override
  final int? permissionCount;
  @override
  final bool? requiresApproval;
  @override
  final bool? hasPendingChange;

  factory _$RoleDefinitionSummaryResponse(
          [void Function(RoleDefinitionSummaryResponseBuilder)? updates]) =>
      (RoleDefinitionSummaryResponseBuilder()..update(updates))._build();

  _$RoleDefinitionSummaryResponse._(
      {this.id,
      this.name,
      this.description,
      this.isSystem,
      this.isActive,
      this.rowVersion,
      this.memberCount,
      this.permissionCount,
      this.requiresApproval,
      this.hasPendingChange})
      : super._();
  @override
  RoleDefinitionSummaryResponse rebuild(
          void Function(RoleDefinitionSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleDefinitionSummaryResponseBuilder toBuilder() =>
      RoleDefinitionSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleDefinitionSummaryResponse &&
        id == other.id &&
        name == other.name &&
        description == other.description &&
        isSystem == other.isSystem &&
        isActive == other.isActive &&
        rowVersion == other.rowVersion &&
        memberCount == other.memberCount &&
        permissionCount == other.permissionCount &&
        requiresApproval == other.requiresApproval &&
        hasPendingChange == other.hasPendingChange;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, description.hashCode);
    _$hash = $jc(_$hash, isSystem.hashCode);
    _$hash = $jc(_$hash, isActive.hashCode);
    _$hash = $jc(_$hash, rowVersion.hashCode);
    _$hash = $jc(_$hash, memberCount.hashCode);
    _$hash = $jc(_$hash, permissionCount.hashCode);
    _$hash = $jc(_$hash, requiresApproval.hashCode);
    _$hash = $jc(_$hash, hasPendingChange.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RoleDefinitionSummaryResponse')
          ..add('id', id)
          ..add('name', name)
          ..add('description', description)
          ..add('isSystem', isSystem)
          ..add('isActive', isActive)
          ..add('rowVersion', rowVersion)
          ..add('memberCount', memberCount)
          ..add('permissionCount', permissionCount)
          ..add('requiresApproval', requiresApproval)
          ..add('hasPendingChange', hasPendingChange))
        .toString();
  }
}

class RoleDefinitionSummaryResponseBuilder
    implements
        Builder<RoleDefinitionSummaryResponse,
            RoleDefinitionSummaryResponseBuilder> {
  _$RoleDefinitionSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _description;
  String? get description => _$this._description;
  set description(String? description) => _$this._description = description;

  bool? _isSystem;
  bool? get isSystem => _$this._isSystem;
  set isSystem(bool? isSystem) => _$this._isSystem = isSystem;

  bool? _isActive;
  bool? get isActive => _$this._isActive;
  set isActive(bool? isActive) => _$this._isActive = isActive;

  String? _rowVersion;
  String? get rowVersion => _$this._rowVersion;
  set rowVersion(String? rowVersion) => _$this._rowVersion = rowVersion;

  int? _memberCount;
  int? get memberCount => _$this._memberCount;
  set memberCount(int? memberCount) => _$this._memberCount = memberCount;

  int? _permissionCount;
  int? get permissionCount => _$this._permissionCount;
  set permissionCount(int? permissionCount) =>
      _$this._permissionCount = permissionCount;

  bool? _requiresApproval;
  bool? get requiresApproval => _$this._requiresApproval;
  set requiresApproval(bool? requiresApproval) =>
      _$this._requiresApproval = requiresApproval;

  bool? _hasPendingChange;
  bool? get hasPendingChange => _$this._hasPendingChange;
  set hasPendingChange(bool? hasPendingChange) =>
      _$this._hasPendingChange = hasPendingChange;

  RoleDefinitionSummaryResponseBuilder() {
    RoleDefinitionSummaryResponse._defaults(this);
  }

  RoleDefinitionSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _description = $v.description;
      _isSystem = $v.isSystem;
      _isActive = $v.isActive;
      _rowVersion = $v.rowVersion;
      _memberCount = $v.memberCount;
      _permissionCount = $v.permissionCount;
      _requiresApproval = $v.requiresApproval;
      _hasPendingChange = $v.hasPendingChange;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleDefinitionSummaryResponse other) {
    _$v = other as _$RoleDefinitionSummaryResponse;
  }

  @override
  void update(void Function(RoleDefinitionSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleDefinitionSummaryResponse build() => _build();

  _$RoleDefinitionSummaryResponse _build() {
    final _$result = _$v ??
        _$RoleDefinitionSummaryResponse._(
          id: id,
          name: name,
          description: description,
          isSystem: isSystem,
          isActive: isActive,
          rowVersion: rowVersion,
          memberCount: memberCount,
          permissionCount: permissionCount,
          requiresApproval: requiresApproval,
          hasPendingChange: hasPendingChange,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
