// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'role_definition_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$RoleDefinitionResponse extends RoleDefinitionResponse {
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
  final DateTime? createdAtUtc;
  @override
  final int? memberCount;
  @override
  final int? pendingAssignmentRequestCount;
  @override
  final bool? requiresApproval;
  @override
  final BuiltList<RoleGrantResponse>? permissions;
  @override
  final RoleChangeRequestResponse? pendingChange;

  factory _$RoleDefinitionResponse(
          [void Function(RoleDefinitionResponseBuilder)? updates]) =>
      (RoleDefinitionResponseBuilder()..update(updates))._build();

  _$RoleDefinitionResponse._(
      {this.id,
      this.name,
      this.description,
      this.isSystem,
      this.isActive,
      this.rowVersion,
      this.createdAtUtc,
      this.memberCount,
      this.pendingAssignmentRequestCount,
      this.requiresApproval,
      this.permissions,
      this.pendingChange})
      : super._();
  @override
  RoleDefinitionResponse rebuild(
          void Function(RoleDefinitionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  RoleDefinitionResponseBuilder toBuilder() =>
      RoleDefinitionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is RoleDefinitionResponse &&
        id == other.id &&
        name == other.name &&
        description == other.description &&
        isSystem == other.isSystem &&
        isActive == other.isActive &&
        rowVersion == other.rowVersion &&
        createdAtUtc == other.createdAtUtc &&
        memberCount == other.memberCount &&
        pendingAssignmentRequestCount == other.pendingAssignmentRequestCount &&
        requiresApproval == other.requiresApproval &&
        permissions == other.permissions &&
        pendingChange == other.pendingChange;
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
    _$hash = $jc(_$hash, createdAtUtc.hashCode);
    _$hash = $jc(_$hash, memberCount.hashCode);
    _$hash = $jc(_$hash, pendingAssignmentRequestCount.hashCode);
    _$hash = $jc(_$hash, requiresApproval.hashCode);
    _$hash = $jc(_$hash, permissions.hashCode);
    _$hash = $jc(_$hash, pendingChange.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'RoleDefinitionResponse')
          ..add('id', id)
          ..add('name', name)
          ..add('description', description)
          ..add('isSystem', isSystem)
          ..add('isActive', isActive)
          ..add('rowVersion', rowVersion)
          ..add('createdAtUtc', createdAtUtc)
          ..add('memberCount', memberCount)
          ..add('pendingAssignmentRequestCount', pendingAssignmentRequestCount)
          ..add('requiresApproval', requiresApproval)
          ..add('permissions', permissions)
          ..add('pendingChange', pendingChange))
        .toString();
  }
}

class RoleDefinitionResponseBuilder
    implements Builder<RoleDefinitionResponse, RoleDefinitionResponseBuilder> {
  _$RoleDefinitionResponse? _$v;

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

  DateTime? _createdAtUtc;
  DateTime? get createdAtUtc => _$this._createdAtUtc;
  set createdAtUtc(DateTime? createdAtUtc) =>
      _$this._createdAtUtc = createdAtUtc;

  int? _memberCount;
  int? get memberCount => _$this._memberCount;
  set memberCount(int? memberCount) => _$this._memberCount = memberCount;

  int? _pendingAssignmentRequestCount;
  int? get pendingAssignmentRequestCount =>
      _$this._pendingAssignmentRequestCount;
  set pendingAssignmentRequestCount(int? pendingAssignmentRequestCount) =>
      _$this._pendingAssignmentRequestCount = pendingAssignmentRequestCount;

  bool? _requiresApproval;
  bool? get requiresApproval => _$this._requiresApproval;
  set requiresApproval(bool? requiresApproval) =>
      _$this._requiresApproval = requiresApproval;

  ListBuilder<RoleGrantResponse>? _permissions;
  ListBuilder<RoleGrantResponse> get permissions =>
      _$this._permissions ??= ListBuilder<RoleGrantResponse>();
  set permissions(ListBuilder<RoleGrantResponse>? permissions) =>
      _$this._permissions = permissions;

  RoleChangeRequestResponseBuilder? _pendingChange;
  RoleChangeRequestResponseBuilder get pendingChange =>
      _$this._pendingChange ??= RoleChangeRequestResponseBuilder();
  set pendingChange(RoleChangeRequestResponseBuilder? pendingChange) =>
      _$this._pendingChange = pendingChange;

  RoleDefinitionResponseBuilder() {
    RoleDefinitionResponse._defaults(this);
  }

  RoleDefinitionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _name = $v.name;
      _description = $v.description;
      _isSystem = $v.isSystem;
      _isActive = $v.isActive;
      _rowVersion = $v.rowVersion;
      _createdAtUtc = $v.createdAtUtc;
      _memberCount = $v.memberCount;
      _pendingAssignmentRequestCount = $v.pendingAssignmentRequestCount;
      _requiresApproval = $v.requiresApproval;
      _permissions = $v.permissions?.toBuilder();
      _pendingChange = $v.pendingChange?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(RoleDefinitionResponse other) {
    _$v = other as _$RoleDefinitionResponse;
  }

  @override
  void update(void Function(RoleDefinitionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  RoleDefinitionResponse build() => _build();

  _$RoleDefinitionResponse _build() {
    _$RoleDefinitionResponse _$result;
    try {
      _$result = _$v ??
          _$RoleDefinitionResponse._(
            id: id,
            name: name,
            description: description,
            isSystem: isSystem,
            isActive: isActive,
            rowVersion: rowVersion,
            createdAtUtc: createdAtUtc,
            memberCount: memberCount,
            pendingAssignmentRequestCount: pendingAssignmentRequestCount,
            requiresApproval: requiresApproval,
            permissions: _permissions?.build(),
            pendingChange: _pendingChange?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'permissions';
        _permissions?.build();
        _$failedField = 'pendingChange';
        _pendingChange?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'RoleDefinitionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
