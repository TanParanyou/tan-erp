// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'quotation_document_response.dart';

// **************************************************************************
// BuiltValueGenerator
// **************************************************************************

class _$QuotationDocumentResponse extends QuotationDocumentResponse {
  @override
  final String? number;
  @override
  final DateTime? issuedAtUtc;
  @override
  final String? currency;
  @override
  final String? locale;
  @override
  final bool? hasIncompleteTranslations;
  @override
  final QuotationCustomerResponse? customer;
  @override
  final BuiltList<QuotationSectionResponse>? sections;
  @override
  final QuotationTotalsResponse? totals;

  factory _$QuotationDocumentResponse(
          [void Function(QuotationDocumentResponseBuilder)? updates]) =>
      (QuotationDocumentResponseBuilder()..update(updates))._build();

  _$QuotationDocumentResponse._(
      {this.number,
      this.issuedAtUtc,
      this.currency,
      this.locale,
      this.hasIncompleteTranslations,
      this.customer,
      this.sections,
      this.totals})
      : super._();
  @override
  QuotationDocumentResponse rebuild(
          void Function(QuotationDocumentResponseBuilder) updates) =>
      (toBuilder()..update(updates)).build();

  @override
  QuotationDocumentResponseBuilder toBuilder() =>
      QuotationDocumentResponseBuilder()..replace(this);

  @override
  bool operator ==(Object other) {
    if (identical(other, this)) return true;
    return other is QuotationDocumentResponse &&
        number == other.number &&
        issuedAtUtc == other.issuedAtUtc &&
        currency == other.currency &&
        locale == other.locale &&
        hasIncompleteTranslations == other.hasIncompleteTranslations &&
        customer == other.customer &&
        sections == other.sections &&
        totals == other.totals;
  }

  @override
  int get hashCode {
    var _$hash = 0;
    _$hash = $jc(_$hash, number.hashCode);
    _$hash = $jc(_$hash, issuedAtUtc.hashCode);
    _$hash = $jc(_$hash, currency.hashCode);
    _$hash = $jc(_$hash, locale.hashCode);
    _$hash = $jc(_$hash, hasIncompleteTranslations.hashCode);
    _$hash = $jc(_$hash, customer.hashCode);
    _$hash = $jc(_$hash, sections.hashCode);
    _$hash = $jc(_$hash, totals.hashCode);
    _$hash = $jf(_$hash);
    return _$hash;
  }

  @override
  String toString() {
    return (newBuiltValueToStringHelper(r'QuotationDocumentResponse')
          ..add('number', number)
          ..add('issuedAtUtc', issuedAtUtc)
          ..add('currency', currency)
          ..add('locale', locale)
          ..add('hasIncompleteTranslations', hasIncompleteTranslations)
          ..add('customer', customer)
          ..add('sections', sections)
          ..add('totals', totals))
        .toString();
  }
}

class QuotationDocumentResponseBuilder
    implements
        Builder<QuotationDocumentResponse, QuotationDocumentResponseBuilder> {
  _$QuotationDocumentResponse? _$v;

  String? _number;
  String? get number => _$this._number;
  set number(String? number) => _$this._number = number;

  DateTime? _issuedAtUtc;
  DateTime? get issuedAtUtc => _$this._issuedAtUtc;
  set issuedAtUtc(DateTime? issuedAtUtc) => _$this._issuedAtUtc = issuedAtUtc;

  String? _currency;
  String? get currency => _$this._currency;
  set currency(String? currency) => _$this._currency = currency;

  String? _locale;
  String? get locale => _$this._locale;
  set locale(String? locale) => _$this._locale = locale;

  bool? _hasIncompleteTranslations;
  bool? get hasIncompleteTranslations => _$this._hasIncompleteTranslations;
  set hasIncompleteTranslations(bool? hasIncompleteTranslations) =>
      _$this._hasIncompleteTranslations = hasIncompleteTranslations;

  QuotationCustomerResponseBuilder? _customer;
  QuotationCustomerResponseBuilder get customer =>
      _$this._customer ??= QuotationCustomerResponseBuilder();
  set customer(QuotationCustomerResponseBuilder? customer) =>
      _$this._customer = customer;

  ListBuilder<QuotationSectionResponse>? _sections;
  ListBuilder<QuotationSectionResponse> get sections =>
      _$this._sections ??= ListBuilder<QuotationSectionResponse>();
  set sections(ListBuilder<QuotationSectionResponse>? sections) =>
      _$this._sections = sections;

  QuotationTotalsResponseBuilder? _totals;
  QuotationTotalsResponseBuilder get totals =>
      _$this._totals ??= QuotationTotalsResponseBuilder();
  set totals(QuotationTotalsResponseBuilder? totals) => _$this._totals = totals;

  QuotationDocumentResponseBuilder() {
    QuotationDocumentResponse._defaults(this);
  }

  QuotationDocumentResponseBuilder get _$this {
    final $v = _$v;
    if ($v != null) {
      _number = $v.number;
      _issuedAtUtc = $v.issuedAtUtc;
      _currency = $v.currency;
      _locale = $v.locale;
      _hasIncompleteTranslations = $v.hasIncompleteTranslations;
      _customer = $v.customer?.toBuilder();
      _sections = $v.sections?.toBuilder();
      _totals = $v.totals?.toBuilder();
      _$v = null;
    }
    return this;
  }

  @override
  void replace(QuotationDocumentResponse other) {
    _$v = other as _$QuotationDocumentResponse;
  }

  @override
  void update(void Function(QuotationDocumentResponseBuilder)? updates) {
    if (updates != null) updates(this);
  }

  @override
  QuotationDocumentResponse build() => _build();

  _$QuotationDocumentResponse _build() {
    _$QuotationDocumentResponse _$result;
    try {
      _$result = _$v ??
          _$QuotationDocumentResponse._(
            number: number,
            issuedAtUtc: issuedAtUtc,
            currency: currency,
            locale: locale,
            hasIncompleteTranslations: hasIncompleteTranslations,
            customer: _customer?.build(),
            sections: _sections?.build(),
            totals: _totals?.build(),
          );
    } catch (_) {
      late String _$failedField;
      try {
        _$failedField = 'customer';
        _customer?.build();
        _$failedField = 'sections';
        _sections?.build();
        _$failedField = 'totals';
        _totals?.build();
      } catch (e) {
        throw BuiltValueNestedFieldError(
            r'QuotationDocumentResponse', _$failedField, e.toString());
      }
      rethrow;
    }
    replace(_$result);
    return _$result;
  }
}

// ignore_for_file: deprecated_member_use_from_same_package,type=lint
