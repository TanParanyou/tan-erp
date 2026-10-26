//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_totals_response.g.dart';

/// QuotationTotalsResponse
///
/// Properties:
/// * [subtotal] 
/// * [discountType] 
/// * [discountValue] 
/// * [discountAmount] 
/// * [netBeforeTax] 
/// * [taxAmount] 
/// * [grandTotal] 
@BuiltValue()
abstract class QuotationTotalsResponse implements Built<QuotationTotalsResponse, QuotationTotalsResponseBuilder> {
  @BuiltValueField(wireName: r'subtotal')
  double? get subtotal;

  @BuiltValueField(wireName: r'discountType')
  String? get discountType;

  @BuiltValueField(wireName: r'discountValue')
  double? get discountValue;

  @BuiltValueField(wireName: r'discountAmount')
  double? get discountAmount;

  @BuiltValueField(wireName: r'netBeforeTax')
  double? get netBeforeTax;

  @BuiltValueField(wireName: r'taxAmount')
  double? get taxAmount;

  @BuiltValueField(wireName: r'grandTotal')
  double? get grandTotal;

  QuotationTotalsResponse._();

  factory QuotationTotalsResponse([void updates(QuotationTotalsResponseBuilder b)]) = _$QuotationTotalsResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationTotalsResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationTotalsResponse> get serializer => _$QuotationTotalsResponseSerializer();
}

class _$QuotationTotalsResponseSerializer implements PrimitiveSerializer<QuotationTotalsResponse> {
  @override
  final Iterable<Type> types = const [QuotationTotalsResponse, _$QuotationTotalsResponse];

  @override
  final String wireName = r'QuotationTotalsResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationTotalsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.subtotal != null) {
      yield r'subtotal';
      yield serializers.serialize(
        object.subtotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.discountType != null) {
      yield r'discountType';
      yield serializers.serialize(
        object.discountType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.discountValue != null) {
      yield r'discountValue';
      yield serializers.serialize(
        object.discountValue,
        specifiedType: const FullType(double),
      );
    }
    if (object.discountAmount != null) {
      yield r'discountAmount';
      yield serializers.serialize(
        object.discountAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.netBeforeTax != null) {
      yield r'netBeforeTax';
      yield serializers.serialize(
        object.netBeforeTax,
        specifiedType: const FullType(double),
      );
    }
    if (object.taxAmount != null) {
      yield r'taxAmount';
      yield serializers.serialize(
        object.taxAmount,
        specifiedType: const FullType(double),
      );
    }
    if (object.grandTotal != null) {
      yield r'grandTotal';
      yield serializers.serialize(
        object.grandTotal,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationTotalsResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationTotalsResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'subtotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.subtotal = valueDes;
          break;
        case r'discountType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.discountType = valueDes;
          break;
        case r'discountValue':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountValue = valueDes;
          break;
        case r'discountAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.discountAmount = valueDes;
          break;
        case r'netBeforeTax':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.netBeforeTax = valueDes;
          break;
        case r'taxAmount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxAmount = valueDes;
          break;
        case r'grandTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grandTotal = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationTotalsResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationTotalsResponseBuilder();
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


