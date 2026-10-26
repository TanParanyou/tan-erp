// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'branch_deactivation_check_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$BranchDeactivationCheckResponse
    extends BranchDeactivationCheckResponse {
  @override
  final bool? canDeactivate;
  @override
  final BuiltList<BranchBlockerResponse>? blockers;

  factory _$BranchDeactivationCheckResponse(
          [void Function(BranchDeactivationCheckResponseBuilder)? updates]) =>
      (BranchDeactivationCheckResponseBuilder()..update(updates))._build();

  _$BranchDeactivationCheckResponse._({this.canDeactivate, this.blockers})
      : super._();
  @override
  BranchDeactivationCheckResponse rebuild(
          void Function(BranchDeactivationCheckResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  BranchDeactivationCheckResponseBuilder toBuilder() =>
      BranchDeactivationCheckResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is BranchDeactivationCheckResponse &&
        canDeactivate == other.canDeactivate &&
        blockers == other.blockers;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, canDeactivate.hashCode);
    _$hash = $jc(_$hash, blockers.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'BranchDeactivationCheckResponse')
          ..add('canDeactivate', canDeactivate)
          ..add('blockers', blockers))
        .toString();
  }
}

class BranchDeactivationCheckResponseBuilder
    implements
        Builder<BranchDeactivationCheckResponse,
            BranchDeactivationCheckResponseBuilder> {
  _$BranchDeactivationCheckResponse? _$v;

  bool? _canDeactivate;
  bool? get canDeactivate => _$this._canDeactivate;
  set canDeactivate(bool? canDeactivate) =>
      _$this._canDeactivate = canDeactivate;

  ListBuilder<BranchBlockerResponse>? _blockers;
  ListBuilder<BranchBlockerResponse> get blockers =>
      _$this._blockers ??= ListBuilder<BranchBlockerResponse>();
  set blockers(ListBuilder<BranchBlockerResponse>? blockers) =>
      _$this._blockers = blockers;

  BranchDeactivationCheckResponseBuilder() {
    BranchDeactivationCheckResponse._defaults(this);
  }

  BranchDeactivationCheckResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _canDeactivate = $v.canDeactivate;
      _blockers = $v.blockers?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(BranchDeactivationCheckResponse other) {
    _$v = other as _$BranchDeactivationCheckResponse;
  }

  @override
  void update(void Function(BranchDeactivationCheckResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  BranchDeactivationCheckResponse build() => _build();

  _$BranchDeactivationCheckResponse _build() {
    _$BranchDeactivationCheckResponse _$result;
    try {
      _$result = _$v ??
          _$BranchDeactivationCheckResponse._(
            canDeactivate: canDeactivate,
            blockers: _blockers?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'blockers';
        _blockers?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'BranchDeactivationCheckResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
