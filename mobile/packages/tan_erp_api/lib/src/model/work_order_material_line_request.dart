//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_material_line_request.g.dart';

/// WorkOrderMaterialLineRequest
///
/// Properties:
/// * [itemId] 
/// * [quantity] 
@BuiltValue()
abstract class WorkOrderMaterialLineRequest implements Built<WorkOrderMaterialLineRequest, WorkOrderMaterialLineRequestBuilder> {
  @BuiltValueField(wireName: r'itemId')
  String? get itemId;

  @BuiltValueField(wireName: r'quantity')
  double? get quantity;

  WorkOrderMaterialLineRequest._();

  factory WorkOrderMaterialLineRequest([void updates(WorkOrderMaterialLineRequestBuilder b)]) = _$WorkOrderMaterialLineRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderMaterialLineRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderMaterialLineRequest> get serializer => _$WorkOrderMaterialLineRequestSerializer();
}

class _$WorkOrderMaterialLineRequestSerializer implements PrimitiveSerializer<WorkOrderMaterialLineRequest> {
  @override
  final Iterable<Type> types = const [WorkOrderMaterialLineRequest, _$WorkOrderMaterialLineRequest];

  @override
  final String wireName = r'WorkOrderMaterialLineRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderMaterialLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.itemId != null) {
      yield r'itemId';
      yield serializers.serialize(
        object.itemId,
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
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderMaterialLineRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderMaterialLineRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'itemId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.itemId = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.quantity = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderMaterialLineRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderMaterialLineRequestBuilder();
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


