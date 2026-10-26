//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_work_item_response.g.dart';

/// QuotationWorkItemResponse
///
/// Properties:
/// * [code] 
/// * [description] 
/// * [descriptionTh] 
/// * [descriptionEn] 
/// * [quantity] 
/// * [unitCode] 
/// * [unitPrice] 
/// * [lineTotal] 
@BuiltValue()
abstract class QuotationWorkItemResponse implements Built<QuotationWorkItemResponse, QuotationWorkItemResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'descriptionTh')
  String? get descriptionTh;

  @BuiltValueField(wireName: r'descriptionEn')
  String? get descriptionEn;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'unitCode')
  String? get unitCode;

  @BuiltValueField(wireName: r'unitPrice')
  double? get unitPrice;

  @BuiltValueField(wireName: r'lineTotal')
  double? get lineTotal;

  QuotationWorkItemResponse._();

  factory QuotationWorkItemResponse([void updates(QuotationWorkItemResponseBuilder b)]) = _$QuotationWorkItemResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationWorkItemResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationWorkItemResponse> get serializer => _$QuotationWorkItemResponseSerializer();
}

class _$QuotationWorkItemResponseSerializer implements PrimitiveSerializer<QuotationWorkItemResponse> {
  @override
  final Iterable<Type> types = const [QuotationWorkItemResponse, _$QuotationWorkItemResponse];

  @override
  final String wireName = r'QuotationWorkItemResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationWorkItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.descriptionTh != null) {
      yield r'descriptionTh';
      yield serializers.serialize(
        object.descriptionTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.descriptionEn != null) {
      yield r'descriptionEn';
      yield serializers.serialize(
        object.descriptionEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.unitCode != null) {
      yield r'unitCode';
      yield serializers.serialize(
        object.unitCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.unitPrice != null) {
      yield r'unitPrice';
      yield serializers.serialize(
        object.unitPrice,
        specifiedType: const FullType(double),
      );
    }
    if (object.lineTotal != null) {
      yield r'lineTotal';
      yield serializers.serialize(
        object.lineTotal,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationWorkItemResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationWorkItemResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'descriptionTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.descriptionTh = valueDes;
          break;
        case r'descriptionEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.descriptionEn = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.unitCode = valueDes;
          break;
        case r'unitPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.unitPrice = valueDes;
          break;
        case r'lineTotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.lineTotal = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationWorkItemResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationWorkItemResponseBuilder();
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


