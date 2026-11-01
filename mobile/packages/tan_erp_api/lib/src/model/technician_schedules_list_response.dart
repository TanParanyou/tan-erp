//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/technician_assignment_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'technician_schedules_list_response.g.dart';

/// TechnicianSchedulesListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class TechnicianSchedulesListResponse implements Built<TechnicianSchedulesListResponse, TechnicianSchedulesListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<TechnicianAssignmentResponse>? get items;

  TechnicianSchedulesListResponse._();

  factory TechnicianSchedulesListResponse([void updates(TechnicianSchedulesListResponseBuilder b)]) = _$TechnicianSchedulesListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(TechnicianSchedulesListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<TechnicianSchedulesListResponse> get serializer => _$TechnicianSchedulesListResponseSerializer();
}

class _$TechnicianSchedulesListResponseSerializer implements PrimitiveSerializer<TechnicianSchedulesListResponse> {
  @override
  final Iterable<Type> types = const [TechnicianSchedulesListResponse, _$TechnicianSchedulesListResponse];

  @override
  final String wireName = r'TechnicianSchedulesListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    TechnicianSchedulesListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(TechnicianAssignmentResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    TechnicianSchedulesListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required TechnicianSchedulesListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(TechnicianAssignmentResponse)]),
          ) as BuiltList<TechnicianAssignmentResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  TechnicianSchedulesListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = TechnicianSchedulesListResponseBuilder();
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


