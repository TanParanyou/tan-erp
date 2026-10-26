// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_section_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationSectionResponse extends QuotationSectionResponse {
  @override
  final String? code;
  @override
  final String? name;
  @override
  final String? nameTh;
  @override
  final String? nameEn;
  @override
  final double? subtotal;
  @override
  final BuiltList<QuotationWorkItemResponse>? workItems;

  factory _$QuotationSectionResponse(
          [void Function(QuotationSectionResponseBuilder)? updates]) =>
      (QuotationSectionResponseBuilder()..update(updates))._build();

  _$QuotationSectionResponse._(
      {this.code,
      this.name,
      this.nameTh,
      this.nameEn,
      this.subtotal,
      this.workItems})
      : super._();
  @override
  QuotationSectionResponse rebuild(
          void Function(QuotationSectionResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationSectionResponseBuilder toBuilder() =>
      QuotationSectionResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationSectionResponse &&
        code == other.code &&
        name == other.name &&
        nameTh == other.nameTh &&
        nameEn == other.nameEn &&
        subtotal == other.subtotal &&
        workItems == other.workItems;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, code.hashCode);
    _$hash = $jc(_$hash, name.hashCode);
    _$hash = $jc(_$hash, nameTh.hashCode);
    _$hash = $jc(_$hash, nameEn.hashCode);
    _$hash = $jc(_$hash, subtotal.hashCode);
    _$hash = $jc(_$hash, workItems.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationSectionResponse')
          ..add('code', code)
          ..add('name', name)
          ..add('nameTh', nameTh)
          ..add('nameEn', nameEn)
          ..add('subtotal', subtotal)
          ..add('workItems', workItems))
        .toString();
  }
}

class QuotationSectionResponseBuilder
    implements
        Builder<QuotationSectionResponse, QuotationSectionResponseBuilder> {
  _$QuotationSectionResponse? _$v;

  String? _code;
  String? get code => _$this._code;
  set code(String? code) => _$this._code = code;

  String? _name;
  String? get name => _$this._name;
  set name(String? name) => _$this._name = name;

  String? _nameTh;
  String? get nameTh => _$this._nameTh;
  set nameTh(String? nameTh) => _$this._nameTh = nameTh;

  String? _nameEn;
  String? get nameEn => _$this._nameEn;
  set nameEn(String? nameEn) => _$this._nameEn = nameEn;

  double? _subtotal;
  double? get subtotal => _$this._subtotal;
  set subtotal(double? subtotal) => _$this._subtotal = subtotal;

  ListBuilder<QuotationWorkItemResponse>? _workItems;
  ListBuilder<QuotationWorkItemResponse> get workItems =>
      _$this._workItems ??= ListBuilder<QuotationWorkItemResponse>();
  set workItems(ListBuilder<QuotationWorkItemResponse>? workItems) =>
      _$this._workItems = workItems;

  QuotationSectionResponseBuilder() {
    QuotationSectionResponse._defaults(this);
  }

  QuotationSectionResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _code = $v.code;
      _name = $v.name;
      _nameTh = $v.nameTh;
      _nameEn = $v.nameEn;
      _subtotal = $v.subtotal;
      _workItems = $v.workItems?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationSectionResponse other) {
    _$v = other as _$QuotationSectionResponse;
  }

  @override
  void update(void Function(QuotationSectionResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationSectionResponse build() => _build();

  _$QuotationSectionResponse _build() {
    _$QuotationSectionResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationSectionResponse._(
            code: code,
            name: name,
            nameTh: nameTh,
            nameEn: nameEn,
            subtotal: subtotal,
            workItems: _workItems?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'workItems';
        _workItems?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuotationSectionResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
