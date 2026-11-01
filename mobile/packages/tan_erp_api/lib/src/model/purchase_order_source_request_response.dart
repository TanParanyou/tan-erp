//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_order_source_request_response.g.dart';

/// PurchaseOrderSourceRequestResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [convertedAtUtc] 
@BuiltValue()
abstract class PurchaseOrderSourceRequestResponse implements Built<PurchaseOrderSourceRequestResponse, PurchaseOrderSourceRequestResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'convertedAtUtc')
  DateTime? get convertedAtUtc;

  PurchaseOrderSourceRequestResponse._();

  factory PurchaseOrderSourceRequestResponse([void updates(PurchaseOrderSourceRequestResponseBuilder b)]) = _$PurchaseOrderSourceRequestResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseOrderSourceRequestResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseOrderSourceRequestResponse> get serializer => _$PurchaseOrderSourceRequestResponseSerializer();
}

class _$PurchaseOrderSourceRequestResponseSerializer implements PrimitiveSerializer<PurchaseOrderSourceRequestResponse> {
  @override
  final Iterable<Type> types = const [PurchaseOrderSourceRequestResponse, _$PurchaseOrderSourceRequestResponse];

  @override
  final String wireName = r'PurchaseOrderSourceRequestResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseOrderSourceRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.convertedAtUtc != null) {
      yield r'convertedAtUtc';
      yield serializers.serialize(
        object.convertedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseOrderSourceRequestResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseOrderSourceRequestResponseBuilder result,
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
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'convertedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.convertedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseOrderSourceRequestResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseOrderSourceRequestResponseBuilder();
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


