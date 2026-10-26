//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_line_request.g.dart';

/// BomLineRequest
///
/// Properties:
/// * [componentItemId] 
/// * [quantity] 
/// * [scrapPercent] 
@BuiltValue()
abstract class BomLineRequest implements Built<BomLineRequest, BomLineRequestBuilder> {
  @BuiltValueField(wireName: r'componentItemId')
  String? get componentItemId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  @BuiltValueField(wireName: r'scrapPercent')
  double? get scrapPercent;

  BomLineRequest._();

  factory BomLineRequest([void updates(BomLineRequestBuilder b)]) = _$BomLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomLineRequest> get serializer => _$BomLineRequestSerializer();
}

class _$BomLineRequestSerializer implements PrimitiveSerializer<BomLineRequest> {
  @override
  final Iterable<Type> types = const [BomLineRequest, _$BomLineRequest];

  @override
  final String wireName = r'BomLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.componentItemId != null) {
      yield r'componentItemId';
      yield serializers.serialize(
        object.componentItemId,
        specifiedType: const FullType(String),
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
  }

  @override
  Object serialize(
    Serializers serializers,
    BomLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'componentItemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.componentItemId = valueDes;
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
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomLineRequestBuilder();
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


