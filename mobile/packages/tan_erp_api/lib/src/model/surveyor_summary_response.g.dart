// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'surveyor_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$SurveyorSummaryResponse extends SurveyorSummaryResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;

  factory _$SurveyorSummaryResponse(
          [void Function(SurveyorSummaryResponseBuilder)? updates]) =>
      (SurveyorSummaryResponseBuilder()..update(updates))._build();

  _$SurveyorSummaryResponse._({this.id, this.displayName, this.email})
      : super._();
  @override
  SurveyorSummaryResponse rebuild(
          void Function(SurveyorSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  SurveyorSummaryResponseBuilder toBuilder() =>
      SurveyorSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is SurveyorSummaryResponse &&
        id == other.id &&
        displayName == other.displayName &&
        email == other.email;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, id.hashCode);
    _$hash = $jc(_$hash, displayName.hashCode);
    _$hash = $jc(_$hash, email.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'SurveyorSummaryResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email))
        .toString();
  }
}

class SurveyorSummaryResponseBuilder
    implements
        Builder<SurveyorSummaryResponse, SurveyorSummaryResponseBuilder> {
  _$SurveyorSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  SurveyorSummaryResponseBuilder() {
    SurveyorSummaryResponse._defaults(this);
  }

  SurveyorSummaryResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _id = $v.id;
      _displayName = $v.displayName;
      _email = $v.email;
      _$v = null;
    }
    return this;
  }

  @override
  void replace(SurveyorSummaryResponse other) {
    _$v = other as _$SurveyorSummaryResponse;
  }

  @override
  void update(void Function(SurveyorSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  SurveyorSummaryResponse build() => _build();

  _$SurveyorSummaryResponse _build() {
    final _$result = _$v ??
        _$SurveyorSummaryResponse._(
          id: id,
          displayName: displayName,
          email: email,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
