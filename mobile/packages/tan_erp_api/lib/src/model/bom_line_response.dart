//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/production_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_line_response.g.dart';

/// BomLineResponse
///
/// Properties:
/// * [id] 
/// * [component] 
/// * [quantity] 
/// * [scrapPercent] 
/// * [grossQuantity] 
@BuiltValue()
abstract class BomLineResponse implements Built<BomLineResponse, BomLineResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'component')
  ProductionItemResponse? get component;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'scrapPercent')
  double? get scrapPercent;

  @BuiltValueField(wireName: r'grossQuantity')
  double? get grossQuantity;

  BomLineResponse._();

  factory BomLineResponse([void updates(BomLineResponseBuilder b)]) = _$BomLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomLineResponse> get serializer => _$BomLineResponseSerializer();
}

class _$BomLineResponseSerializer implements PrimitiveSerializer<BomLineResponse> {
  @override
  final Iterable<Type> types = const [BomLineResponse, _$BomLineResponse];

  @override
  final String wireName = r'BomLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.component != null) {
      yield r'component';
      yield serializers.serialize(
        object.component,
        specifiedType: const FullType(ProductionItemResponse),
      );
    }
    if (object.quantity != null) {
      yield r'quantity';
      yield serializers.serialize(
        object.quantity,
        specifiedType: const FullType(double),
      );
    }
    if (object.scrapPercent != null) {
      yield r'scrapPercent';
      yield serializers.serialize(
        object.scrapPercent,
        specifiedType: const FullType(double),
      );
    }
    if (object.grossQuantity != null) {
      yield r'grossQuantity';
      yield serializers.serialize(
        object.grossQuantity,
        specifiedType: const FullType(double),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomLineResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'component':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(ProductionItemResponse),
          ) as ProductionItemResponse?;
          if (valueDes == null) continue;
          result.component.replace(valueDes);
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        case r'scrapPercent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.scrapPercent = valueDes;
          break;
        case r'grossQuantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.grossQuantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomLineResponseBuilder();
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


