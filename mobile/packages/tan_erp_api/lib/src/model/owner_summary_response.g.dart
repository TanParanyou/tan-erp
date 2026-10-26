// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'owner_summary_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$OwnerSummaryResponse extends OwnerSummaryResponse {
  @override
  final String? id;
  @override
  final String? displayName;
  @override
  final String? email;

  factory _$OwnerSummaryResponse(
          [void Function(OwnerSummaryResponseBuilder)? updates]) =>
      (OwnerSummaryResponseBuilder()..update(updates))._build();

  _$OwnerSummaryResponse._({this.id, this.displayName, this.email}) : super._();
  @override
  OwnerSummaryResponse rebuild(
          void Function(OwnerSummaryResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  OwnerSummaryResponseBuilder toBuilder() =>
      OwnerSummaryResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is OwnerSummaryResponse &&
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
    return (newBuiltValueToStringHelper(r'OwnerSummaryResponse')
          ..add('id', id)
          ..add('displayName', displayName)
          ..add('email', email))
        .toString();
  }
}

class OwnerSummaryResponseBuilder
    implements Builder<OwnerSummaryResponse, OwnerSummaryResponseBuilder> {
  _$OwnerSummaryResponse? _$v;

  String? _id;
  String? get id => _$this._id;
  set id(String? id) => _$this._id = id;

  String? _displayName;
  String? get displayName => _$this._displayName;
  set displayName(String? displayName) => _$this._displayName = displayName;

  String? _email;
  String? get email => _$this._email;
  set email(String? email) => _$this._email = email;

  OwnerSummaryResponseBuilder() {
    OwnerSummaryResponse._defaults(this);
  }

  OwnerSummaryResponseBuilder get _$this {
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
  void replace(OwnerSummaryResponse other) {
    _$v = other as _$OwnerSummaryResponse;
  }

  @override
  void update(void Function(OwnerSummaryResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  OwnerSummaryResponse build() => _build();

  _$OwnerSummaryResponse _build() {
    final _$result = _$v ??
        _$OwnerSummaryResponse._(
          id: id,
          displayName: displayName,
          email: email,
        );
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
