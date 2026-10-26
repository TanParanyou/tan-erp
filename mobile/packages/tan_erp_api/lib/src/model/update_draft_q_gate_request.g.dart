// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'update_draft_q_gate_request.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$UpdateDraftQGateRequest extends UpdateDraftQGateRequest {
  @override
  final String? scopeSummary;
  @override
  final BuiltList<String>? workTypes;
  @override
  final DateTime? nextActionAtUtc;
  @override
  final String? nextActionNote;

  factory _$UpdateDraftQGateRequest(
          [void Function(UpdateDraftQGateRequestBuilder)? updates]) =>
      (UpdateDraftQGateRequestBuilder()..update(updates))._build();

  _$UpdateDraftQGateRequest._(
      {this.scopeSummary,
      this.workTypes,
      this.nextActionAtUtc,
      this.nextActionNote})
      : super._();
  @override
  UpdateDraftQGateRequest rebuild(
          void Function(UpdateDraftQGateRequestBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  UpdateDraftQGateRequestBuilder toBuilder() =>
      UpdateDraftQGateRequestBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is UpdateDraftQGateRequest &&
        scopeSummary == other.scopeSummary &&
        workTypes == other.workTypes &&
        nextActionAtUtc == other.nextActionAtUtc &&
        nextActionNote == other.nextActionNote;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, scopeSummary.hashCode);
    _$hash = $jc(_$hash, workTypes.hashCode);
    _$hash = $jc(_$hash, nextActionAtUtc.hashCode);
    _$hash = $jc(_$hash, nextActionNote.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'UpdateDraftQGateRequest')
          ..add('scopeSummary', scopeSummary)
          ..add('workTypes', workTypes)
          ..add('nextActionAtUtc', nextActionAtUtc)
          ..add('nextActionNote', nextActionNote))
        .toString();
  }
}

class UpdateDraftQGateRequestBuilder
    implements
        Builder<UpdateDraftQGateRequest, UpdateDraftQGateRequestBuilder> {
  _$UpdateDraftQGateRequest? _$v;

  String? _scopeSummary;
  String? get scopeSummary => _$this._scopeSummary;
  set scopeSummary(String? scopeSummary) => _$this._scopeSummary = scopeSummary;

  ListBuilder<String>? _workTypes;
  ListBuilder<String> get workTypes =>
      _$this._workTypes ??= ListBuilder<String>();
  set workTypes(ListBuilder<String>? workTypes) =>
      _$this._workTypes = workTypes;

  DateTime? _nextActionAtUtc;
  DateTime? get nextActionAtUtc => _$this._nextActionAtUtc;
  set nextActionAtUtc(DateTime? nextActionAtUtc) =>
      _$this._nextActionAtUtc = nextActionAtUtc;

  String? _nextActionNote;
  String? get nextActionNote => _$this._nextActionNote;
  set nextActionNote(String? nextActionNote) =>
      _$this._nextActionNote = nextActionNote;

  UpdateDraftQGateRequestBuilder() {
    UpdateDraftQGateRequest._defaults(this);
  }

  UpdateDraftQGateRequestBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _scopeSummary = $v.scopeSummary;
      _workTypes = $v.workTypes?.toBuilder();
      _nextActionAtUtc = $v.nextActionAtUtc;
      _nextActionNote = $v.nextActionNote;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(UpdateDraftQGateRequest other) {
    _$v = other as _$UpdateDraftQGateRequest;
  }

  @override
  void update(void Function(UpdateDraftQGateRequestBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  UpdateDraftQGateRequest build() => _build();

  _$UpdateDraftQGateRequest _build() {
    _$UpdateDraftQGateRequest _$result;
    try {
      _$result = _$v ??
          _$UpdateDraftQGateRequest._(
            scopeSummary: scopeSummary,
            workTypes: _workTypes?.build(),
            nextActionAtUtc: nextActionAtUtc,
            nextActionNote: nextActionNote,
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workTypes';
        _workTypes?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'UpdateDraftQGateRequest', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
