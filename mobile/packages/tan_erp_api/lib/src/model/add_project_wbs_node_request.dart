//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'add_project_wbs_node_request.g.dart';

/// AddProjectWbsNodeRequest
///
/// Properties:
/// * [parentId] 
/// * [code] 
/// * [name] 
@BuiltValue()
abstract class AddProjectWbsNodeRequest implements Built<AddProjectWbsNodeRequest, AddProjectWbsNodeRequestBuilder> {
  @BuiltValueField(wireName: r'parentId')
  String? get parentId;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  AddProjectWbsNodeRequest._();

  factory AddProjectWbsNodeRequest([void updates(AddProjectWbsNodeRequestBuilder b)]) = _$AddProjectWbsNodeRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AddProjectWbsNodeRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AddProjectWbsNodeRequest> get serializer => _$AddProjectWbsNodeRequestSerializer();
}

class _$AddProjectWbsNodeRequestSerializer implements PrimitiveSerializer<AddProjectWbsNodeRequest> {
  @override
  final Iterable<Type> types = const [AddProjectWbsNodeRequest, _$AddProjectWbsNodeRequest];

  @override
  final String wireName = r'AddProjectWbsNodeRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AddProjectWbsNodeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.parentId != null) {
      yield r'parentId';
      yield serializers.serialize(
        object.parentId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AddProjectWbsNodeRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AddProjectWbsNodeRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'parentId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.parentId = valueDes;
          break;
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AddProjectWbsNodeRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AddProjectWbsNodeRequestBuilder();
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


