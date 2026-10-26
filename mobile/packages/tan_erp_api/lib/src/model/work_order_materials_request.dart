//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_order_material_line_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_order_materials_request.g.dart';

/// WorkOrderMaterialsRequest
///
/// Properties:
/// * [lines] 
@BuiltValue()
abstract class WorkOrderMaterialsRequest implements Built<WorkOrderMaterialsRequest, WorkOrderMaterialsRequestBuilder> {
  @BuiltValueField(wireName: r'lines')
  BuiltList<WorkOrderMaterialLineRequest>? get lines;

  WorkOrderMaterialsRequest._();

  factory WorkOrderMaterialsRequest([void updates(WorkOrderMaterialsRequestBuilder b)]) = _$WorkOrderMaterialsRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkOrderMaterialsRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkOrderMaterialsRequest> get serializer => _$WorkOrderMaterialsRequestSerializer();
}

class _$WorkOrderMaterialsRequestSerializer implements PrimitiveSerializer<WorkOrderMaterialsRequest> {
  @override
  final Iterable<Type> types = const [WorkOrderMaterialsRequest, _$WorkOrderMaterialsRequest];

  @override
  final String wireName = r'WorkOrderMaterialsRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkOrderMaterialsRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderMaterialLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkOrderMaterialsRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkOrderMaterialsRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(WorkOrderMaterialLineRequest)]),
          ) as BuiltList<WorkOrderMaterialLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkOrderMaterialsRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkOrderMaterialsRequestBuilder();
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


