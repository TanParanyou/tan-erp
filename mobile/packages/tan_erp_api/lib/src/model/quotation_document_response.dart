//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/quotation_section_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quotation_customer_response.dart';
import 'package:tan_erp_api/src/model/quotation_totals_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_document_response.g.dart';

/// QuotationDocumentResponse
///
/// Properties:
/// * [number] 
/// * [issuedAtUtc] 
/// * [currency] 
/// * [locale] 
/// * [hasIncompleteTranslations] 
/// * [customer] 
/// * [sections] 
/// * [totals] 
@BuiltValue()
abstract class QuotationDocumentResponse implements Built<QuotationDocumentResponse, QuotationDocumentResponseBuilder> {
  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'issuedAtUtc')
  DateTime? get issuedAtUtc;

  @BuiltValueField(wireName: r'currency')
  String? get currency;

  @BuiltValueField(wireName: r'locale')
  String? get locale;

  @BuiltValueField(wireName: r'hasIncompleteTranslations')
  bool? get hasIncompleteTranslations;

  @BuiltValueField(wireName: r'customer')
  QuotationCustomerResponse? get customer;

  @BuiltValueField(wireName: r'sections')
  BuiltList<QuotationSectionResponse>? get sections;

  @BuiltValueField(wireName: r'totals')
  QuotationTotalsResponse? get totals;

  QuotationDocumentResponse._();

  factory QuotationDocumentResponse([void updates(QuotationDocumentResponseBuilder b)]) = _$QuotationDocumentResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationDocumentResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationDocumentResponse> get serializer => _$QuotationDocumentResponseSerializer();
}

class _$QuotationDocumentResponseSerializer implements PrimitiveSerializer<QuotationDocumentResponse> {
  @override
  final Iterable<Type> types = const [QuotationDocumentResponse, _$QuotationDocumentResponse];

  @override
  final String wireName = r'QuotationDocumentResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationDocumentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.issuedAtUtc != null) {
      yield r'issuedAtUtc';
      yield serializers.serialize(
        object.issuedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.currency != null) {
      yield r'currency';
      yield serializers.serialize(
        object.currency,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.locale != null) {
      yield r'locale';
      yield serializers.serialize(
        object.locale,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.hasIncompleteTranslations != null) {
      yield r'hasIncompleteTranslations';
      yield serializers.serialize(
        object.hasIncompleteTranslations,
        specifiedType: const FullType(bool),
      );
    }
    if (object.customer != null) {
      yield r'customer';
      yield serializers.serialize(
        object.customer,
        specifiedType: const FullType(QuotationCustomerResponse),
      );
    }
    if (object.sections != null) {
      yield r'sections';
      yield serializers.serialize(
        object.sections,
        specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationSectionResponse)]),
      );
    }
    if (object.totals != null) {
      yield r'totals';
      yield serializers.serialize(
        object.totals,
        specifiedType: const FullType(QuotationTotalsResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationDocumentResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationDocumentResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'issuedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.issuedAtUtc = valueDes;
          break;
        case r'currency':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.currency = valueDes;
          break;
        case r'locale':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.locale = valueDes;
          break;
        case r'hasIncompleteTranslations':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasIncompleteTranslations = valueDes;
          break;
        case r'customer':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationCustomerResponse),
          ) as QuotationCustomerResponse?;
          if (valueDes == null) continue;
          result.customer.replace(valueDes);
          break;
        case r'sections':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationSectionResponse)]),
          ) as BuiltList<QuotationSectionResponse>?;
          if (valueDes == null) continue;
          result.sections.replace(valueDes);
          break;
        case r'totals':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(QuotationTotalsResponse),
          ) as QuotationTotalsResponse?;
          if (valueDes == null) continue;
          result.totals.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationDocumentResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationDocumentResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


