// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'set_branch_availability_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SetBranchAvailabilityRequest extends SetBranchAvailabilityRequest {
  @override
  final String mode;
  @override
  final BuiltList<String>? branchIds;

  factory _$SetBranchAvailabilityRequest(
          [void Function(SetBranchAvailabilityRequestBuilder)? updates]) =>
      (SetBranchAvailabilityRequestBuilder()..update(updates))._build();

  _$SetBranchAvailabilityRequest._({required this.mode, this.branchIds})
      : super._();
  @override
  SetBranchAvailabilityRequest rebuild(
          void Function(SetBranchAvailabilityRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SetBranchAvailabilityRequestBuilder toBuilder() =>
      SetBranchAvailabilityRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SetBranchAvailabilityRequest &&
        mode == other.mode &&
        branchIds == other.branchIds;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, mode.hashCode);
    _$hash = $jc(_$hash, branchIds.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SetBranchAvailabilityRequest')
          ..add('mode', mode)
          ..add('branchIds', branchIds))
        .toString();
  }
}

class SetBranchAvailabilityRequestBuilder
    implements
        Builder<SetBranchAvailabilityRequest,
            SetBranchAvailabilityRequestBuilder> {
  _$SetBranchAvailabilityRequest? _$v;

  String? _mode;
  String? get mode => _$this._mode;
  set mode(String? mode) => _$this._mode = mode;

  ListBuilder<String>? _branchIds;
  ListBuilder<String> get branchIds =>
      _$this._branchIds ??= ListBuilder<String>();
  set branchIds(ListBuilder<String>? branchIds) =>
      _$this._branchIds = branchIds;

  SetBranchAvailabilityRequestBuilder() {
    SetBranchAvailabilityRequest._defaults(this);
  }

  SetBranchAvailabilityRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _mode = $v.mode;
      _branchIds = $v.branchIds?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SetBranchAvailabilityRequest other) {
    _$v = other as _$SetBranchAvailabilityRequest;
  }

  @override
  void update(void Function(SetBranchAvailabilityRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SetBranchAvailabilityRequest build() => _build();

  _$SetBranchAvailabilityRequest _build() {
    _$SetBranchAvailabilityRequest _$result;
    try {
      _$result = _$v ??
          _$SetBranchAvailabilityRequest._(
            mode: BuiltValueNullFieldError.checkNotNull(
                mode, r'SetBranchAvailabilityRequest', 'mode'),
            branchIds: _branchIds?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'branchIds';
        _branchIds?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'SetBranchAvailabilityRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
